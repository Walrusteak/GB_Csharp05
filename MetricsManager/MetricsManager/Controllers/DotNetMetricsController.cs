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

        [HttpGet("agent/{agentId}")]
        public IActionResult GetAllMetricsFromAgent([FromRoute] int agentId)
        {
            _logger.LogInformation($"{GetType().Name}.GetAllMetricsFromAgent: agentId = {agentId}");
            IList<DotNetMetric> metrics = _repository.GetAllByAgentId(agentId);
            AllDotNetMetricsApiResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAgent: agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");
            IList<DotNetMetric> metrics = _repository.GetByTimePeriod(agentId, fromTime, toTime);
            AllDotNetMetricsApiResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster")]
        public IActionResult GetAllMetricsFromCluster()
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster");
            IList<DotNetMetric> metrics = _repository.GetAll();
            AllDotNetMetricsApiResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster: fromTime = {fromTime}; toTime = {toTime}");
            IList<DotNetMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllDotNetMetricsApiResponse response = new()
            {
                Metrics = new List<DotNetMetricDto>()
            };
            foreach (DotNetMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<DotNetMetricDto>(metric));
            }
            return Ok(response);
        }
    }
}
