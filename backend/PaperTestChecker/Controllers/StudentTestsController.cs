using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperTestChecker.DTOs;
using PaperTestChecker.Services;

namespace PaperTestChecker.Controllers;

[ApiController]
[Route("api/student-tests")]
[Authorize]
public class StudentTestsController : ControllerBase
{
    private readonly IStudentTestService _service;

    public StudentTestsController(IStudentTestService service)
    {
        _service = service;
    }

    /// <summary>List tests assigned to the current student</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyTests()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var tests = await _service.GetTestsForStudentAsync(userId.Value);
        return Ok(tests);
    }

    /// <summary>Get test to take (no correct answers)</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTest(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var test = await _service.GetTestForTakingAsync(id, userId.Value);
        return test is null ? NotFound() : Ok(test);
    }

    /// <summary>Submit answers for a test</summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitTest(Guid id, [FromBody] SubmitTestDto dto)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            var result = await _service.SubmitTestAsync(id, userId.Value, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Get a completed attempt result</summary>
    [HttpGet("attempts/{id:guid}")]
    public async Task<IActionResult> GetAttempt(Guid id)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _service.GetAttemptAsync(id, userId.Value);
        return result is null ? NotFound() : Ok(result);
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
