using AutoMapper;
using MetricsManager.DAL;
using MetricsManager.Models;
using MetricsManager.Requests;
using MetricsManager.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MetricsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ILogger<AgentsController> _logger;
        private readonly IAgentsRepository _repository;
        private readonly IMapper _mapper;

        public AgentsController(IAgentsRepository repository, ILogger<AgentsController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, $"NLog встроен в {GetType().Name}");
            _mapper = mapper;
        }

        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentRegisterRequest agentInfo)
        {
            if (_repository.GetByUrl(agentInfo.Url) != null)
                return BadRequest();
            _repository.Create(new()
            {
                Url = agentInfo.Url,
                Enabled = agentInfo.Enabed
            });
            _logger.LogDebug(1, $"Агент {agentInfo.Url} зарегистрирован");
            return Ok();
        }

        [HttpPut("enable/{agentId}")]
        public IActionResult EnableAgentById([FromRoute] int agentId)
        {
            _repository.Update(agentId, true);
            _logger.LogDebug(1, $"Агент {agentId} включен");
            return Ok();
        }

        [HttpPut("disable/{agentId}")]
        public IActionResult DisableAgentById([FromRoute] int agentId)
        {
            _repository.Update(agentId, false);
            _logger.LogDebug(1, $"Агент {agentId} выключен");
            return Ok();
        }

        [HttpGet("agents")]
        public IActionResult GetAgents()
        {
            _logger.LogInformation($"{GetType().Name}.GetAll");
            IList<Agent> agents = _repository.GetAll();
            AllAgentsApiResponse response = new()
            {
                Agents = new List<AgentDto>()
            };
            foreach (Agent agent in agents)
            {
                response.Agents.Add(_mapper.Map<AgentDto>(agent));
            }
            return Ok(response);
        }
    }
}
