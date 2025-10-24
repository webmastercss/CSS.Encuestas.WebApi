namespace CSS.Encuestas.Infrastructure.Options;
public record SmtpOptions
{
    public required string Host { get; init; } 
    public int Port { get; init; } = 587;
    public required string Password { get; init; } 
    public required string FromAddress { get; init; } 
    public string? Bcc { get; init; }
    public string FromName { get; init; } = string.Empty;

}