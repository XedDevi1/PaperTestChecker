using PaperTestChecker.DTOs;

namespace PaperTestChecker.Services;

public interface ITeacherService
{
    Task<List<StudentInfoDto>> GetStudentsAsync();
    Task<List<StudentQuestionDto>> GetStudentQuestionsAsync(Guid studentId);
    Task<GeneratedTestDto> GenerateTestAsync(Guid teacherId, GenerateTestDto dto);
    Task<List<GeneratedTestSummaryDto>> GetGeneratedTestsAsync(Guid teacherId);
    Task<GeneratedTestDto?> GetGeneratedTestAsync(Guid testId);
    Task<List<TeacherTestAttemptDto>> GetTestAttemptsAsync(Guid teacherId);
    Task<TestAttemptDetailDto?> GetTestAttemptDetailAsync(Guid attemptId);
}
