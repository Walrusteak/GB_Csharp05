using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class AllAgentsApiResponse
    {
        public List<AgentDto> Agents { get; set; }
    }

    public class AgentDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool Enabled { get; set; }
    }
}
