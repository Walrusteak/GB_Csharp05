using MetricsManagerClient.Models;
using System.Collections.Generic;

namespace MetricsManagerClient.Responses
{
    internal class RamMetricApiResponse
    {
        public List<RamMetric> Metrics { get; set; }
    }
}
