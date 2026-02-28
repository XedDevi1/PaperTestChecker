using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperTestChecker.DTOs;
using PaperTestChecker.Services;

namespace PaperTestChecker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        if (result is null)
            return Conflict(new { error = "Email already in use" });

        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        if (result is null)
            return Unauthorized(new { error = "Invalid credentials" });

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        var name = User.FindFirst("name")?.Value;

        if (id is null)
            return Unauthorized();

        return Ok(new { id, email, role, name });
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok(new { access = "admin" });
    }

    [Authorize(Policy = "TeacherOrAdmin")]
    [HttpGet("teacher-test")]
    public IActionResult TeacherTest()
    {
        return Ok(new { access = "teacher-or-admin" });
    }
}
