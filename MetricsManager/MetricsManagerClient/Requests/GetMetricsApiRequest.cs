using System;

namespace MetricsManagerClient.Requests
{
    internal class GetMetricsApiRequest
    {
        public int AgentId { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
}
