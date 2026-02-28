namespace PaperTestChecker.Models;

public class QuestionResult
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string StudentAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public string[] RecommendedReadings { get; set; } = [];

    public Submission Submission { get; set; } = null!;
}
