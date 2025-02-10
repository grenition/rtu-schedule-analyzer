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

        var lessons = await _lessonsService.GetAllLessons(searchKey, cancellationToken);

        return Ok(lessons);
    }
    
    [HttpGet("inconviences")]
    public async Task<IActionResult> GetAllInconveniences([FromQuery] string? searchKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(searchKey))
            return BadRequest("Search Key is invalid!");

        var lessons = await _lessonsService.GetAllLessons(searchKey, cancellationToken);
        var conveniences = _lessonsService.GetInconveniences(lessons);

        return Ok(conveniences);
    }
}
