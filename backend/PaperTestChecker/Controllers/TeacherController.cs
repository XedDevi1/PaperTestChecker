using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperTestChecker.DTOs;
using PaperTestChecker.Services;

namespace PaperTestChecker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TeacherOrAdmin")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacher;

    public TeacherController(ITeacherService teacher)
    {
        _teacher = teacher;
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _teacher.GetStudentsAsync();
        return Ok(students);
    }

    [HttpGet("students/{id:guid}/questions")]
    public async Task<IActionResult> GetStudentQuestions(Guid id)
    {
        var questions = await _teacher.GetStudentQuestionsAsync(id);
        return Ok(questions);
    }

    [HttpPost("generate-test")]
    public async Task<IActionResult> GenerateTest([FromBody] GenerateTestDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var test = await _teacher.GenerateTestAsync(userId.Value, dto);
            return Ok(test);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("tests")]
    public async Task<IActionResult> GetTests()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var tests = await _teacher.GetGeneratedTestsAsync(userId.Value);
        return Ok(tests);
    }

    [HttpGet("tests/{id:guid}")]
    public async Task<IActionResult> GetTest(Guid id)
    {
        var test = await _teacher.GetGeneratedTestAsync(id);
        return test is null ? NotFound() : Ok(test);
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetResults()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var attempts = await _teacher.GetTestAttemptsAsync(userId.Value);
        return Ok(attempts);
    }

    [HttpGet("results/{id:guid}")]
    public async Task<IActionResult> GetResultDetail(Guid id)
    {
        var detail = await _teacher.GetTestAttemptDetailAsync(id);
        return detail is null ? NotFound() : Ok(detail);
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
