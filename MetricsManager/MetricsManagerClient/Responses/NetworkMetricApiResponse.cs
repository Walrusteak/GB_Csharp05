using MetricsManagerClient.Models;
using System.Collections.Generic;

namespace MetricsManagerClient.Responses
{
    internal class NetworkMetricApiResponse
    {
        public List<NetworkMetric> Metrics { get; set; }
    }
}
