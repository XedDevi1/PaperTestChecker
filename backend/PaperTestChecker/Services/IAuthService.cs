using PaperTestChecker.DTOs;

namespace PaperTestChecker.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(UserLoginDto dto);
    Task<List<UserInfoDto>> GetAllUsersAsync();
}
