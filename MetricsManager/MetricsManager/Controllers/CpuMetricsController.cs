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

        /// <summary>
        /// Получает метрики CPU заданного агента за весь диапазон времени
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        /// GET api/metrics/cpu/agent/1
        ///
        /// </remarks>
        /// <param name="agentId">Номер агента</param>
        /// <returns>Список метрик ЦП заданного агента за все время</returns>
        /// <response code="201">Если всё хорошо</response>
        /// <response code="400">Если передали неправильные параметры</response>
        [HttpGet("agent/{agentId}")]
        public IActionResult GetAllMetricsFromAgent([FromRoute] int agentId)
        {
            _logger.LogInformation($"{GetType().Name}.GetAllMetricsFromAgent: agentId = {agentId}");
            IList<CpuMetric> metrics = _repository.GetAllByAgentId(agentId);
            AllCpuMetricsApiResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAgent: agentId = {agentId}; fromTime = {fromTime}; toTime = {toTime}");
            IList<CpuMetric> metrics = _repository.GetByTimePeriod(agentId, fromTime, toTime);
            AllCpuMetricsApiResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster")]
        public IActionResult GetAllMetricsFromCluster()
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster");
            IList<CpuMetric> metrics = _repository.GetAll();
            AllCpuMetricsApiResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromCluster([FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            _logger.LogInformation($"{GetType().Name}.GetMetricsFromAllCluster: fromTime = {fromTime}; toTime = {toTime}");
            IList<CpuMetric> metrics = _repository.GetByTimePeriod(fromTime, toTime);
            AllCpuMetricsApiResponse response = new()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (CpuMetric metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }
            return Ok(response);
        }
    }
}
    