using FlurlWithPolly.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlurlWithPolly.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var rng = new Random();
            var randomInteger = rng.Next(0, 8);

            switch (randomInteger)
            {
                case 0:
                    Debug.WriteLine("Webservice:About to serve a 404...");
                    return StatusCode(StatusCodes.Status404NotFound);

                case 1:
                    Debug.WriteLine("Webservice:About to serve a 503...");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);

                case 2:
                    Debug.WriteLine("Webservice:Sleeping for 10 seconds then serving a 504...");
                    Task.Delay(10000);
                    Debug.WriteLine("Webservice:About to serve a 504...");

                    return StatusCode(StatusCodes.Status504GatewayTimeout);

                case 3:
                    Debug.WriteLine("Webservice:About to serve a 401...");
                    return StatusCode(StatusCodes.Status401Unauthorized);

                default:
                    {
                        var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                        {
                            Date = DateTime.Now.AddDays(index),
                            TemperatureC = rng.Next(-20, 55),
                            Summary = Summaries[rng.Next(Summaries.Length)]
                        }).ToArray();

                        Debug.WriteLine("Webservice:About to correctly serve a 200 response");

                        return Ok(list);
                    }
            }
        }
    }
}
