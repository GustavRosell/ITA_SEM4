using Microsoft.AspNetCore.Mvc;

namespace CBService.Controllers;

[ApiController]
[Route("[controller]")]
public class CBServiceController : ControllerBase
{

    private readonly ILogger<CBServiceController> _logger;
    private readonly IConfiguration _config;

    public CBServiceController(ILogger<CBServiceController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [HttpGet("GetService")]
    [ProducesResponseType(typeof((string hostname, int time)), 200)]
    public IActionResult GetService()
    {
        var isFail = _config["ToFail"] == "yes";
        string hostname = System.Net.Dns.GetHostName();
        _logger.LogInformation($"Fail condition for {hostname} is set to {isFail}");
        var seconds = DateTime.Now.Second;
        var hasError = (seconds > 30 && seconds < 45);
        if (hasError && isFail)
        {
            return StatusCode(503);
        }
        else
        {
            return Ok(new { hostname, seconds });
        }
    }
}
