using Microsoft.EntityFrameworkCore;
using PaperTestChecker.Data;
using PaperTestChecker.DTOs;
using PaperTestChecker.Models;
using PaperTestChecker.Services;

namespace PaperTestChecker.Tests.Unit;

public class StudentTestServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(AppDbContext db, StudentTestService service, Guid teacherId, Guid studentId, Guid testId)> SeedAsync()
    {
        var db = CreateDb();
        var service = new StudentTestService(db);

        var teacher = new User { Id = Guid.NewGuid(), Name = "Teacher", Email = "t@test.com", Role = "teacher", PasswordHash = "", CreatedAt = DateTime.UtcNow };
        var student = new User { Id = Guid.NewGuid(), Name = "Student", Email = "s@test.com", Role = "student", PasswordHash = "", CreatedAt = DateTime.UtcNow };
        db.Users.AddRange(teacher, student);

        var test = new GeneratedTest
        {
            Id = Guid.NewGuid(),
            Title = "Test Alpha",
            CreatedByUserId = teacher.Id,
            ForStudentUserId = student.Id,
            CreatedAt = DateTime.UtcNow,
            Items = new List<GeneratedTestItem>
            {
                new() { Id = Guid.NewGuid(), QuestionNumber = 1, QuestionText = "Q1?", Options = new[] { "A", "B", "C", "D" }, CorrectAnswer = "A" },
                new() { Id = Guid.NewGuid(), QuestionNumber = 2, QuestionText = "Q2?", Options = new[] { "X", "Y", "Z", "W" }, CorrectAnswer = "X" },
            }
        };
        db.GeneratedTests.Add(test);
        await db.SaveChangesAsync();

        return (db, service, teacher.Id, student.Id, test.Id);
    }

    private static SubmitTestDto MakeSubmit(params (int q, string a)[] answers)
        => new() { Answers = answers.Select(x => new SubmitAnswerDto { QuestionNumber = x.q, SelectedAnswer = x.a }).ToList() };

    [Fact]
    public async Task GetTestsForStudent_ReturnsOnlyStudentsTests()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();

        var tests = await service.GetTestsForStudentAsync(studentId);

        Assert.Single(tests);
        Assert.Equal(testId, tests[0].Id);
    }

    [Fact]
    public async Task GetTestsForStudent_OtherStudentSeesNoTests()
    {
        var (_, service, _, _, _) = await SeedAsync();

        var tests = await service.GetTestsForStudentAsync(Guid.NewGuid());

        Assert.Empty(tests);
    }

    [Fact]
    public async Task GetTestForTaking_ReturnsQuestionsWithOptions()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();

        var testDto = await service.GetTestForTakingAsync(testId, studentId);

        Assert.NotNull(testDto);
        Assert.Equal(2, testDto.Questions.Count);
        Assert.All(testDto.Questions, q => Assert.NotEmpty(q.Options));
    }

    [Fact]
    public async Task SubmitTest_CorrectAnswers_ScoresFullMarks()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();

        var result = await service.SubmitTestAsync(testId, studentId, MakeSubmit((1, "A"), (2, "X")));

        Assert.Equal(2, result.Score);
        Assert.Equal(2, result.MaxScore);
        Assert.All(result.Answers, a => Assert.True(a.IsCorrect));
    }

    [Fact]
    public async Task SubmitTest_WrongAnswers_ScoresZero()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();

        var result = await service.SubmitTestAsync(testId, studentId, MakeSubmit((1, "D"), (2, "W")));

        Assert.Equal(0, result.Score);
        Assert.All(result.Answers, a => Assert.False(a.IsCorrect));
    }

    [Fact]
    public async Task SubmitTest_ReSubmission_ThrowsInvalidOperationException()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();
        await service.SubmitTestAsync(testId, studentId, MakeSubmit((1, "A"), (2, "X")));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SubmitTestAsync(testId, studentId, MakeSubmit((1, "A"), (2, "X"))));
    }

    [Fact]
    public async Task GetAttempt_WrongStudent_ReturnsNull()
    {
        var (_, service, _, studentId, testId) = await SeedAsync();
        var attempt = await service.SubmitTestAsync(testId, studentId, MakeSubmit((1, "A"), (2, "X")));

        var result = await service.GetAttemptAsync(attempt.Id, Guid.NewGuid());

        Assert.Null(result);
    }
}
