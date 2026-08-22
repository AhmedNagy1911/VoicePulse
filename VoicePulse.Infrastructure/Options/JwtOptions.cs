namespace VoicePulse.Infrastructure.Options;

public class JwtOptions
{
    public static string SectionName = "Jwt";

    public string Key {  get; init; } = string.Empty;
    public string Issuer {  get; init; } = string.Empty;
    public string Audience {  get; init; } = string.Empty;

    public int ExpiryMinutes { get; init; }
}
