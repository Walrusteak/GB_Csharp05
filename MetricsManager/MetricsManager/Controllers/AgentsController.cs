using Microsoft.AspNetCore.Mvc;
using System;
namespace MetricsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
        {
            return Ok();
        }

        [HttpPut("enable/{agentId}")]
        public IActionResult EnableAgentById([FromRoute] int agentId)
        {
            return Ok();
        }

        [HttpPut("disable/{agentId}")]
        public IActionResult DisableAgentById([FromRoute] int agentId)
        {
            return Ok();
        }

        [HttpGet("agents")]
        public IActionResult GetAgents()
        {
            return Ok();
        }
    }

    public class AgentInfo
    {
        public int AgentId { get; }
        public Uri AgentAddress { get; }
    }
}
