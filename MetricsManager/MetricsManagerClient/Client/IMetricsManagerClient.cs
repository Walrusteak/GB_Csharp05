using MetricsManagerClient.Models;
using MetricsManagerClient.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetricsManagerClient.Client
{
    internal interface IMetricsManagerClient
    {
        Task<List<Agent>> GetAgents();
        Task<List<IMetric>> GetCpuMetrics(GetMetricsApiRequest request);
        Task<List<IMetric>> GetRamMetrics(GetMetricsApiRequest request);
        Task<List<IMetric>> GetHddMetrics(GetMetricsApiRequest request);
        Task<List<IMetric>> GetNetworkMetrics(GetMetricsApiRequest request);
        Task<List<IMetric>> GetDotNetMetrics(GetMetricsApiRequest request);
    }
}
