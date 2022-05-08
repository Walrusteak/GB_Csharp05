using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class AllDotNetMetricsApiResponse
    {
        public List<DotNetMetricDto> Metrics { get; set; }
    }

    public class DotNetMetricDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int Value { get; set; }
        public TimeSpan Time { get; set; }
    }
}
