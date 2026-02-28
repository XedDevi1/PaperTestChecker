namespace PaperTestChecker.Models;

public class Submission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImageFileName { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public string AnalysisJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public List<QuestionResult> QuestionResults { get; set; } = [];
}
