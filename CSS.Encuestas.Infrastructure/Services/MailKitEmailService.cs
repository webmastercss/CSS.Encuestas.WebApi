using CSS.Encuestas.Application.Interfaces.Services;
using CSS.Encuestas.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CSS.Encuestas.Infrastructure.Services;

public sealed class MailKitEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    public async Task SendAsync(string[] to,string? bcc, string subject, string htmlBody, IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments = null)
    {

        if (to is null || to.Length==0) throw new ArgumentException("Destino requerido.", nameof(to));

        if (string.IsNullOrWhiteSpace(subject)) subject = "(sin asunto)";
        if (string.IsNullOrWhiteSpace(htmlBody)) htmlBody = "<p>(sin contenido)</p>";

        // Construir el mensaje
        var message = new MimeMessage();

        // From
        var from = string.IsNullOrWhiteSpace(_options.FromName)
            ? new MailboxAddress(_options.FromAddress, _options.FromAddress)
            : new MailboxAddress(_options.FromName, _options.FromAddress);
        message.From.Add(from);

        if(!string.IsNullOrWhiteSpace(bcc))
            message.Bcc.Add(new MailboxAddress("Bcc", bcc));

        if (!string.IsNullOrWhiteSpace(_options.Bcc))
            message.Bcc.Add(new MailboxAddress("Bcc", bcc));

        foreach (var addr in to)
        {
            try
            {
                // Validar formato usando MailAddress
                var mailAddress = new System.Net.Mail.MailAddress(addr);

                // Si no lanza excepción, se considera válido
                message.To.Add(MailboxAddress.Parse(mailAddress.Address));
            }
            catch 
            {

            }
        }

            message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            // Alternativa de texto plano mínima (opcional)
            TextBody = StripBasicHtml(htmlBody)
        };

        // Adjuntos (si hay)
        if (attachments != null)
        {
            foreach (var (fileName, content, contentType) in attachments)
            {
                if (content is null || content.Length == 0) continue;
                var ctParsed = string.IsNullOrWhiteSpace(contentType)
                    ? ContentType.Parse("application/octet-stream")
                    : ContentType.Parse(contentType);

                builder.Attachments.Add(fileName ?? "adjunto",
                    new MemoryStream(content, writable: false), ctParsed);
            }
        }

        message.Body = builder.ToMessageBody();

        // Envío
        using var client = new SmtpClient();
        // Si tu servidor usa certificados self-signed en dev:
        // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
        // Si el servidor no soporta autenticación anónima:
        if (!string.IsNullOrWhiteSpace(_options.FromAddress))
            await client.AuthenticateAsync(_options.FromAddress, _options.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    // Conversión súper básica de HTML a texto (para cuerpo alterno)
    private static string StripBasicHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var text = html.Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
                       .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
                       .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase)
                       .Replace("</p>", "\n\n", StringComparison.OrdinalIgnoreCase);
        // Quitar etiquetas simples
        bool inTag = false;
        var chars = text.Where(ch =>
        {
            if (ch == '<') { inTag = true; return false; }
            if (ch == '>') { inTag = false; return false; }
            return !inTag;
        }).ToArray();
        return new string(chars).Trim();
    }
}
