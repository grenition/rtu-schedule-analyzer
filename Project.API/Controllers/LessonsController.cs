using Microsoft.AspNetCore.Mvc;
using Project.Core.Interfaces.Services;

namespace Project.API.Controllers;

[Route("api/lessons")]
[ApiController]
public class LessonsController(ILessonsService lessonsService) : ControllerBase
{
    private readonly ILessonsService _lessonsService = lessonsService;

    [HttpGet("all")]
    public async Task<IActionResult> GetAllLessons([FromQuery] string? searchKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(searchKey))
            return BadRequest("Search Key is invalid!");

        var lessons = await _lessonsService.SearchAllLessons(searchKey, cancellationToken);

        return Ok(lessons);
    }
    
    [HttpGet("inconviences")]
    public async Task<IActionResult> GetAllInconveniences([FromQuery] string? searchKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(searchKey))
            return BadRequest("Search Key is invalid!");

        var inconveniences = await _lessonsService.SearchInconveniences(searchKey, cancellationToken);

        return Ok(inconveniences);
    }

    [HttpPost("clearCache")]
    public async Task<IActionResult> ClearCache([FromQuery] string? searchKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(searchKey))
            return BadRequest("Search Key is invalid!");

        await lessonsService.ClearCache(searchKey);

        return Ok();
    }
}
