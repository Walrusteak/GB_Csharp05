using System;

namespace MetricsManager.Requests
{
    public class GetAllDotNetMetricsApiRequest
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string ClientBaseAddress { get; set; }
    }
}
