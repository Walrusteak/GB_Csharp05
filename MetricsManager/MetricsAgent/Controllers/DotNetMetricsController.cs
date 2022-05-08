using MetricsAgent.DAL;
using MetricsAgent.Models;
using MetricsAgent.Requests;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/dotnet")]
    [ApiController]
    public class DotNetMetricsController : ControllerBase
    {
        private readonly ILogger<DotNetMetricsController> _logger;
        private readonly IDotNetMetricsRepository _repository;
        private readonly IMapper _mapper;

        public DotNetMetricsController(IDotNetMetricsRepository repository, ILogger<DotNetMetricsController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, $"NLog встроен в {GetType().Name}");
            _mapper = mapper;
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetrics([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetrics: fromTime = {fromTime}; toTime = {toTime}");
            IList<DotNetMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllDotNetMetricsResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] DotNetMetricCreateRequest request)
        {
            _logger.LogInformation($"{GetType().Name}.Create: request = {request}");
            _repository.Create(new DotNetMetric
            {
                Time = request.Time,
                Value = request.Value
            });
            return Ok();
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            _logger.LogInformation($"{GetType().Name}.GetAll");
            IList<DotNetMetric> metrics = _repository.GetAll();
            AllDotNetMetricsResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetrics: id = {id}");
            DotNetMetric metric = _repository.GetById(id);
            AllDotNetMetricsResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>(1)
                {
                    _mapper.Map<DotNetMetricDto>(metric)
                }
            };
            return Ok(response);
        }
    }
}
