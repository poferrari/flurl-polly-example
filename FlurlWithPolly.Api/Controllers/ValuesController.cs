using FlurlWithPolly.Api.Acls;
using FlurlWithPolly.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FlurlWithPolly.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly HttpProvider _httpProvider;
        private const string Url = "http://localhost:5000/api/weatherforecast";

        public ValuesController(HttpProvider httpProvider)
        {
            _httpProvider = httpProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var values = await _httpProvider.GetAsync<IEnumerable<WeatherForecast>>(Url);

                Debug.WriteLine("[App]: successful");

                return Ok(values);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[App]: Failed - " + e.Message);
                throw;
            }
        }
    }
}
