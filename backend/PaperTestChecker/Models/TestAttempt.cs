namespace PaperTestChecker.Models;

public class TestAttempt
{
    public Guid Id { get; set; }
    public Guid GeneratedTestId { get; set; }
    public Guid StudentUserId { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public GeneratedTest GeneratedTest { get; set; } = null!;
    public User Student { get; set; } = null!;
    public List<TestAttemptAnswer> Answers { get; set; } = [];
}

public class TestAttemptAnswer
{
    public Guid Id { get; set; }
    public Guid TestAttemptId { get; set; }
    public int QuestionNumber { get; set; }
    public string SelectedAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public TestAttempt TestAttempt { get; set; } = null!;
}
