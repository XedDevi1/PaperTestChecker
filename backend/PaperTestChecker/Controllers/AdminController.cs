using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperTestChecker.DTOs;
using PaperTestChecker.Services;

namespace PaperTestChecker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminController(IAdminService admin)
    {
        _admin = admin;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _admin.GetStatsAsync();
        return Ok(stats);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _admin.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _admin.DeleteUserAsync(id);
        return result ? Ok(new { message = "User deleted" }) : NotFound();
    }

    [HttpPost("users/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _admin.ChangePasswordAsync(dto);
        return result ? Ok(new { message = "Password changed" }) : NotFound();
    }

    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleDto dto)
    {
        var result = await _admin.ChangeRoleAsync(id, dto.Role);
        return result ? Ok(new { message = "Role changed" }) : BadRequest(new { error = "Invalid role or user not found" });
    }
}

public record ChangeRoleDto
{
    public string Role { get; init; } = string.Empty;
}
