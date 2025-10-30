namespace CSS.Encuestas.Application.Interfaces.Services;
public interface IEmailService
{
    Task SendAsync(
        string[] to,
        string? bcc,
        string subject,
        string htmlBody,
        IEnumerable<(string FileName, byte[] Content, string ContentType)>? attachments = null);
}