namespace PaperTestChecker.Models;

public class GeneratedTest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public Guid ForStudentUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User CreatedByUser { get; set; } = null!;
    public User ForStudentUser { get; set; } = null!;
    public List<GeneratedTestItem> Items { get; set; } = [];
}

public class GeneratedTestItem
{
    public Guid Id { get; set; }
    public Guid GeneratedTestId { get; set; }
    public int QuestionNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string[] Options { get; set; } = [];
    public string CorrectAnswer { get; set; } = string.Empty;
    public Guid? SourceQuestionResultId { get; set; }

    public GeneratedTest GeneratedTest { get; set; } = null!;
}
