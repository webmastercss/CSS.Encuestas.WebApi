using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace CSS.Encuestas.Infrastructure.Services;

public sealed class NetMailEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task SendAsync(
        string[] to,
        string? bcc,
        string subject,
        string htmlBody,
        IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments = null)
    {
        if (to is null || to.Length == 0) throw new ArgumentException("Destino requerido.", nameof(to));

        subject ??= "(sin asunto)";
        htmlBody ??= "<p>(sin contenido)</p>";

        // Usuario con el que AUTENTICAS (usa FromAddress si Username viene vacío)
        var userForAuth = _options.FromAddress;

        using var message = new MailMessage();

        // Recomendación: el remitente DEBE ser el mismo que autentica
        var fromDisplay = string.IsNullOrWhiteSpace(_options.FromName) ? userForAuth : _options.FromName;
        message.From = new MailAddress(userForAuth, fromDisplay, Encoding.UTF8);
        // Opcional: si quieres mostrar otro "from" pero el server lo permite:
        // message.Sender = new MailAddress(userForAuth, fromDisplay, Encoding.UTF8);

        foreach (var addr in to)
        {
            if (string.IsNullOrWhiteSpace(addr)) continue;
            try { message.To.Add(new MailAddress(addr)); } catch { /* ignora inválidos */ }
        }

        if (!string.IsNullOrWhiteSpace(bcc)) message.Bcc.Add(new MailAddress(bcc));
        if (!string.IsNullOrWhiteSpace(_options.Bcc)) message.Bcc.Add(new MailAddress(_options.Bcc));

        message.Subject = subject;
        message.SubjectEncoding = Encoding.UTF8;

        // Alternativas texto/HTML
        var altPlain = AlternateView.CreateAlternateViewFromString(
            StripBasicHtml(htmlBody), Encoding.UTF8, MediaTypeNames.Text.Plain);
        var altHtml = AlternateView.CreateAlternateViewFromString(
            htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
        message.AlternateViews.Add(altPlain);
        message.AlternateViews.Add(altHtml);
        message.IsBodyHtml = true;
        message.BodyEncoding = Encoding.UTF8;

        // Adjuntos
        if (attachments is not null)
        {
            foreach (var (fileName, content, contentType) in attachments)
            {
                if (content is null || content.Length == 0) continue;
                var stream = new MemoryStream(content, 0, content.Length, writable: false, publiclyVisible: true);
                var ct = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
                var attachment = new Attachment(stream, fileName ?? "adjunto", ct);
                attachment.ContentDisposition.FileName = fileName ?? "adjunto";
                message.Attachments.Add(attachment);
            }
        }

        // TLS moderno (útil en .NET Framework/ambientes viejos)
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = true, // 587 -> STARTTLS; 465 -> SSL/TLS
            Credentials = new NetworkCredential(userForAuth, _options.Password)
        };

        try
        {
            await client.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            // Verás códigos como GeneralFailure, ClientNotPermitted, MustIssueStartTlsFirst, etc.
            throw new InvalidOperationException(
                $"Fallo SMTP ({ex.StatusCode}): {ex.Message}. Revisa host/puerto/TLS/credenciales y que From coincida con la cuenta.",
                ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No se pudo enviar el correo: {ex.Message}", ex);
        }
    }

    private static string StripBasicHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var text = html
            .Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase)
            .Replace("</p>", "\n\n", StringComparison.OrdinalIgnoreCase);

        bool inTag = false;
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (ch == '<') { inTag = true; continue; }
            if (ch == '>') { inTag = false; continue; }
            if (!inTag) sb.Append(ch);
        }
        return sb.ToString().Trim();
    }
}
