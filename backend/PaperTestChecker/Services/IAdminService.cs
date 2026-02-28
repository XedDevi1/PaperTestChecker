using PaperTestChecker.DTOs;

namespace PaperTestChecker.Services;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();
    Task<List<AdminUserDto>> GetAllUsersAsync();
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
    Task<bool> ChangeRoleAsync(Guid userId, string role);
}
