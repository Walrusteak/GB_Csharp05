using MetricsManagerClient.Models;
using System.Collections.Generic;

namespace MetricsManagerClient.Responses
{
    internal class HddMetricApiResponse
    {
        public List<HddMetric> Metrics { get; set; }
    }
}
