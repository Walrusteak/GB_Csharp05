using MetricsManagerClient.Models;
using MetricsManagerClient.Requests;
using MetricsManagerClient.Responses;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MetricsManagerClient.Client
{
    internal class MetricsManagerClient : IMetricsManagerClient
    {
        private readonly string _metricsManagerAddress;
        private readonly HttpClient _httpClient;

        public MetricsManagerClient(HttpClient httpClient)
        {
            _metricsManagerAddress = (string)App.Current.Resources["MetricsManagerAddress"];
            _httpClient = httpClient;
        }

        public async Task<List<Agent>> GetAgents()
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/agents/agents");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AgentsApiResponse>(response)?.Agents;
            }
            catch
            {
                return null;
            }

        }

        public async Task<List<IMetric>> GetCpuMetrics(GetMetricsApiRequest request)
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/metrics/cpu/agent/{request.AgentId}/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<CpuMetricApiResponse>(response))?.Metrics.Cast<IMetric>().ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IMetric>> GetRamMetrics(GetMetricsApiRequest request)
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/metrics/ram/agent/{request.AgentId}/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<RamMetricApiResponse>(response))?.Metrics.Cast<IMetric>().ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IMetric>> GetHddMetrics(GetMetricsApiRequest request)
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/metrics/hdd/agent/{request.AgentId}/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<HddMetricApiResponse>(response))?.Metrics.Cast<IMetric>().ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IMetric>> GetNetworkMetrics(GetMetricsApiRequest request)
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/metrics/network/agent/{request.AgentId}/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<NetworkMetricApiResponse>(response))?.Metrics.Cast<IMetric>().ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IMetric>> GetDotNetMetrics(GetMetricsApiRequest request)
        {
            HttpRequestMessage? httpRequest = new(HttpMethod.Get, $"{_metricsManagerAddress}/api/metrics/dotnet/agent/{request.AgentId}/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.SendAsync(httpRequest).Result;
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<DotNetMetricApiResponse>(response))?.Metrics.Cast<IMetric>().ToList();
            }
            catch
            {
                return null;
            }
        }
    }
}
