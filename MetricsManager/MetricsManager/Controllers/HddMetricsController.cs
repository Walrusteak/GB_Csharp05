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
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : ControllerBase
    {
        private readonly ILogger<HddMetricsController> _logger;
        private readonly IHddMetricsRepository _repository;
        private readonly IMapper _mapper;

        public HddMetricsController(IHddMetricsRepository repository, ILogger<HddMetricsController> logger, IMapper mapper)
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
            IList<HddMetric> metrics = _repository.GetAllByAgentId(agentId);
            AllHddMetricsApiResponse response = new()
            {
                Metrics = new List<HddMetricDto>()
            };
            foreach (HddMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAgent: agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");
            IList<HddMetric> metrics = _repository.GetByTimePeriod(agentId, fromTime, toTime);
            AllHddMetricsApiResponse response = new()
            {
                Metrics = new List<HddMetricDto>()
            };
            foreach (HddMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster")]
        public IActionResult GetAllMetricsFromCluster()
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster");
            IList<HddMetric> metrics = _repository.GetAll();
            AllHddMetricsApiResponse response = new()
            {
                Metrics = new List<HddMetricDto>()
            };
            foreach (HddMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster: fromTime = {fromTime}; toTime = {toTime}");
            IList<HddMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllHddMetricsApiResponse response = new()
            {
                Metrics = new List<HddMetricDto>()
            };
            foreach (HddMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }
            return Ok(response);
        }
    }
}
