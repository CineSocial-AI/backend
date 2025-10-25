using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace-id";

        _logger.LogInformation("Health check initiated. TraceId: {TraceId}", traceId);

        var response = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "CineSocial API",
            Version = "1.0.0",
            TraceId = traceId
        };

        _logger.LogInformation("Health check completed successfully. Status: {Status}", response.Status);

        return Ok(response);
    }
}
