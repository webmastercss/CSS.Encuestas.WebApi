namespace CSS.Encuestas.Application.Interfaces;
public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default,
        IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments = null);
}