using System;

namespace MetricsManagerClient.Models
{
    public class NetworkMetric : IMetric
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int Value { get; set; }
        public TimeSpan Time { get; set; }
    }
}
