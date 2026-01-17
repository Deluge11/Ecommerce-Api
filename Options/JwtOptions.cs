namespace Options;

public class JwtOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int Lifetime { get; init; }
    public string SigningKey { get; init; }
}
