namespace PaperTestChecker.Services;

public interface IGeminiService
{
    Task<GeminiAnalysisResult> AnalyzeTestImageAsync(byte[] imageBytes, string mimeType);
}

public record GeminiAnalysisResult(
    List<GeminiQuestionResult> Questions,
    int TotalScore,
    int MaxScore);

public record GeminiQuestionResult(
    int QuestionNumber,
    string QuestionText,
    string StudentAnswer,
    string CorrectAnswer,
    bool IsCorrect,
    string Feedback,
    string[] RecommendedReadings);
