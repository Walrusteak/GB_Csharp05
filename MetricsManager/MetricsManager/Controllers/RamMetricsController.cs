using AutoMapper;
using MetricsManager.DAL;
using MetricsManager.Models;
using MetricsManager.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : ControllerBase
    {
        private readonly ILogger<RamMetricsController> _logger;
        private readonly IRamMetricsRepository _repository;
        private readonly IMapper _mapper;

        public RamMetricsController(IRamMetricsRepository repository, ILogger<RamMetricsController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, $"NLog встроен в {GetType().Name}");
            _mapper = mapper;
        }

        [HttpGet("agent/{agentId}")]
        public IActionResult GetAllMetricsFromAgent([FromRoute] int agentId)
        {
            _logger.LogInformation($"{GetType().Name}.GetAllMetricsFromAgent: agentId = {agentId}");
            IList<RamMetric> metrics = _repository.GetAllByAgentId(agentId);
            AllRamMetricsApiResponse response = new()
            {
                Metrics = new List<RamMetricDto>()
            };
            foreach (RamMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAgent: agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");
            IList<RamMetric> metrics = _repository.GetByTimePeriod(agentId, fromTime, toTime);
            AllRamMetricsApiResponse response = new()
            {
                Metrics = new List<RamMetricDto>()
            };
            foreach (RamMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster")]
        public IActionResult GetAllMetricsFromCluster()
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster");
            IList<RamMetric> metrics = _repository.GetAll();
            AllRamMetricsApiResponse response = new()
            {
                Metrics = new List<RamMetricDto>()
            };
            foreach (RamMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster: fromTime = {fromTime}; toTime = {toTime}");
            IList<RamMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllRamMetricsApiResponse response = new()
            {
                Metrics = new List<RamMetricDto>()
            };
            foreach (RamMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }
            return Ok(response);
        }
    }
}
