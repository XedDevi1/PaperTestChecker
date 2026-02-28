using PaperTestChecker.DTOs;

namespace PaperTestChecker.Services;

public interface IStudentTestService
{
    Task<List<StudentTestSummaryDto>> GetTestsForStudentAsync(Guid studentId);
    Task<StudentTestDto?> GetTestForTakingAsync(Guid testId, Guid studentId);
    Task<TestAttemptResultDto> SubmitTestAsync(Guid testId, Guid studentId, SubmitTestDto dto);
    Task<TestAttemptResultDto?> GetAttemptAsync(Guid attemptId, Guid studentId);
}
