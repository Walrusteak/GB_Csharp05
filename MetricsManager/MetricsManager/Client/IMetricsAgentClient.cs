using MetricsManager.Responses;
using MetricsManager.Requests;

namespace MetricsManager.Client
{
    public interface IMetricsAgentClient
    {
        AllCpuMetricsApiResponse GetCpuMetrics(GetAllCpuMetricsApiRequest request);
        AllDotNetMetricsApiResponse GetDotNetMetrics(GetAllDotNetMetricsApiRequest request);
        AllHddMetricsApiResponse GetHddMetrics(GetAllHddMetricsApiRequest request);
        AllNetworkMetricsApiResponse GetNetworkMetrics(GetAllNetworkMetricsApiRequest request);
        AllRamMetricsApiResponse GetRamMetrics(GetAllRamMetricsApiRequest request);
    }
}