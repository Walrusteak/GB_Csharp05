using System;

namespace MetricsManager.Requests
{
    public class GetAllRamMetricsApiRequest
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string ClientBaseAddress { get; set; }
    }
}
