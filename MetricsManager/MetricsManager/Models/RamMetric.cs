using System;

namespace MetricsManager.Models
{
    public class RamMetric
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int Value { get; set; }
        public TimeSpan Time { get; set; }
    }
}
