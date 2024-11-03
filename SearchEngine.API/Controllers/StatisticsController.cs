namespace SearchEngine.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using SearchEngine.Core.Interfaces.Services;
using SearchEngine.Shared.DTOs.Responses;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Gets statistics about indexed documents and terms
    /// </summary>
    /// <returns>Statistics about the search engine's data</returns>
    /// <response code="200">Returns the current statistics</response>
    [HttpGet]
    [ProducesResponseType(typeof(StatisticsResponse), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 300)] // 5 minutes cache on the HTTP level
    public async Task<ActionResult<StatisticsResponse>> GetStatistics()
    {
        var statistics = await _statisticsService.GetStatisticsAsync();
        return Ok(statistics);
    }
} 