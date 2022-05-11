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
    [Route("api/metrics/network")]
    [ApiController]
    public class NetworkMetricsController : ControllerBase
    {
        private readonly ILogger<NetworkMetricsController> _logger;
        private readonly INetworkMetricsRepository _repository;
        private readonly IMapper _mapper;

        public NetworkMetricsController(INetworkMetricsRepository repository, ILogger<NetworkMetricsController> logger, IMapper mapper)
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
            IList<NetworkMetric> metrics = _repository.GetAllByAgentId(agentId);
            AllNetworkMetricsApiResponse response = new()
            {
                Metrics = new List<NetworkMetricDto>()
            };
            foreach (NetworkMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAgent: agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");
            IList<NetworkMetric> metrics = _repository.GetByTimePeriod(agentId, fromTime, toTime);
            AllNetworkMetricsApiResponse response = new()
            {
                Metrics = new List<NetworkMetricDto>()
            };
            foreach (NetworkMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster")]
        public IActionResult GetAllMetricsFromCluster()
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster");
            IList<NetworkMetric> metrics = _repository.GetAll();
            AllNetworkMetricsApiResponse response = new()
            {
                Metrics = new List<NetworkMetricDto>()
            };
            foreach (NetworkMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster: fromTime = {fromTime}; toTime = {toTime}");
            IList<NetworkMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllNetworkMetricsApiResponse response = new()
            {
                Metrics = new List<NetworkMetricDto>()
            };
            foreach (NetworkMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
            }
            return Ok(response);
        }
    }
}
