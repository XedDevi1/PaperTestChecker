namespace PaperTestChecker.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = "PaperTestChecker";
    public string Audience { get; init; } = "PaperTestChecker";
    public int ExpirationDays { get; init; } = 7;
}
