using MetricsManagerClient.Models;
using System.Collections.Generic;

namespace MetricsManagerClient.Responses
{
    internal class CpuMetricApiResponse
    {
        public List<CpuMetric> Metrics { get; set; }
    }
}
