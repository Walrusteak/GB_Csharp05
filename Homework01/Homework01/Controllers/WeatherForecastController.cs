using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
namespace Homework01.Controllers
{
    [ApiController]
    [Route("WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy",
            "Hot", "Sweltering", "Scorching"
        };

        private readonly ValuesHolder _holder;

        public WeatherForecastController(ValuesHolder holder)
        {
            _holder = holder;
        }

        [HttpGet("rng")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody]WeatherForecast weatherForecast)
        {
            _holder.Add(weatherForecast);
            return Ok();
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            return Ok(_holder.Values);
        }

        [HttpGet("read/{from}/{to}")]
        public IActionResult Read([FromRoute] DateTime from, [FromRoute] DateTime to)
        {
            return Ok(_holder.Values.Where(w => w.Date >= from && w.Date <= to).ToList());
        }

        [HttpPut("update")]
        public IActionResult Update([FromQuery] DateTime date, [FromQuery] int temperature)
        {
            WeatherForecast weatherForecast = _holder.Values.FirstOrDefault(w => w.Date == date);
            if (weatherForecast == null)
                return NotFound();
            weatherForecast.TemperatureC = temperature;
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] DateTime date)
        {
            return _holder.Values.RemoveAll(w => w.Date == date) == 0 ? NotFound() : Ok();
        }
    }
}