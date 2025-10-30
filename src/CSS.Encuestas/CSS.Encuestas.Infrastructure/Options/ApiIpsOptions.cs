namespace CSS.Encuestas.Infrastructure.Options;
public sealed record ApiIpsOptions
{
    public string EndPoint { get; init; }
    public string ApiKey { get; init; }
}

