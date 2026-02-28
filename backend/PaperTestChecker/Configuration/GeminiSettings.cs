namespace PaperTestChecker.Configuration;

public class AiSettings
{
    public const string SectionName = "Ai";

    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "meta-llama/llama-4-scout-17b-16e-instruct";
}
