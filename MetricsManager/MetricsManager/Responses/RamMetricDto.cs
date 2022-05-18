using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class AllRamMetricsApiResponse
    {
        public List<RamMetricDto> Metrics { get; set; }
    }

    public class RamMetricDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int Value { get; set; }
        public TimeSpan Time { get; set; }
    }
}
