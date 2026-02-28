using PaperTestChecker.DTOs;

namespace PaperTestChecker.Services;

public interface ISubmissionService
{
    Task<SubmissionResponseDto> AnalyzeTestPhotoAsync(Guid userId, Stream imageStream, string fileName, string contentType);
    Task<SubmissionResponseDto?> GetSubmissionAsync(Guid id, Guid userId);
    Task<List<SubmissionSummaryDto>> GetUserSubmissionsAsync(Guid userId);
}
