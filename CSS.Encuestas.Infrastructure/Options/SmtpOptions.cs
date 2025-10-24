namespace CSS.Encuestas.Infrastructure.Options;
public record SmtpOptions
{
    public string Host { get; init; } = default!;
    public int Port { get; init; } = 587;
    public string Password { get; init; } = default!;
    public string FromAddress { get; init; } = default!;
    public string FromName { get; init; } = string.Empty;

}