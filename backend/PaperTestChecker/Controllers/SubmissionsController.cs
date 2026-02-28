using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperTestChecker.Services;

namespace PaperTestChecker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissions;

    public SubmissionsController(ISubmissionService submissions)
    {
        _submissions = submissions;
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded" });

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new { error = "Only JPEG, PNG, and WebP images are supported" });

        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var result = await _submissions.AnalyzeTestPhotoAsync(
                userId.Value, file.OpenReadStream(), file.FileName, file.ContentType);

            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("rate limit"))
        {
            return StatusCode(429, new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(502, new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMySubmissions()
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var submissions = await _submissions.GetUserSubmissionsAsync(userId.Value);
        return Ok(submissions);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var submission = await _submissions.GetSubmissionAsync(id, userId.Value);
        if (submission is null)
            return NotFound();

        return Ok(submission);
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }
}
