using MetricsManagerClient.Models;
using System.Collections.Generic;

namespace MetricsManagerClient.Responses
{
    internal class DotNetMetricApiResponse
    {
        public List<DotNetMetric> Metrics { get; set; }
    }
}
