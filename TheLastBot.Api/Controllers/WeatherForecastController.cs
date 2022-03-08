using Microsoft.AspNetCore.Mvc;
using TheLastBot.Database.Data;
using TheLastBot.Database.Data.Models;

namespace TheLastBot.Api.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _context;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public List<DiscordUser> Get()
    {
        return _context.DiscordUsers.ToList();
    }
}