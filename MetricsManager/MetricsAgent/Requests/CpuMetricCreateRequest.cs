using System;

namespace MetricsAgent.Requests
{
    public class CpuMetricCreateRequest
    {
        public TimeSpan Time { get; set; }
        public int Value { get; set; }

        public override string ToString() => $"Time = {Time}; Value = {Value}";
    }
}
