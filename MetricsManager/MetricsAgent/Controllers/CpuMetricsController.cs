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
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {
        private readonly ILogger<CpuMetricsController> _logger;
        private readonly ICpuMetricsRepository _repository;
        private readonly IMapper _mapper;

        public CpuMetricsController(ICpuMetricsRepository repository, ILogger<CpuMetricsController> logger, IMapper mapper)
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
            IList<CpuMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllCpuMetricsResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CpuMetricCreateRequest request)
        {
            _logger.LogInformation($"{GetType().Name}.Create: request = {request}");
            _repository.Create(new CpuMetric
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
            IList<CpuMetric> metrics = _repository.GetAll();
            AllCpuMetricsResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("id/{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetrics: id = {id}");
            CpuMetric metric = _repository.GetById(id);
            AllCpuMetricsResponse response = new()
            {
                Metrics = new List<CpuMetricDto>(1)
                {
                    _mapper.Map<CpuMetricDto>(metric)
                }
            };
            return Ok(response);
        }
    }
}
    