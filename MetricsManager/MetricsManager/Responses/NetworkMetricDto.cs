using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class AllNetworkMetricsApiResponse
    {
        public List<NetworkMetricDto> Metrics { get; set; }
    }

    public class NetworkMetricDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int Value { get; set; }
        public TimeSpan Time { get; set; }
    }
}
