using MetricsManager.Requests;
using MetricsManager.Responses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace MetricsManager.Client
{
    public class MetricsAgentClient : IMetricsAgentClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MetricsAgentClient> _logger;

        public MetricsAgentClient(HttpClient httpClient, ILogger<MetricsAgentClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public AllCpuMetricsApiResponse GetCpuMetrics(GetAllCpuMetricsApiRequest request)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/cpu/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<AllCpuMetricsApiResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public AllDotNetMetricsApiResponse GetDotNetMetrics(GetAllDotNetMetricsApiRequest request)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/dotnet/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<AllDotNetMetricsApiResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public AllHddMetricsApiResponse GetHddMetrics(GetAllHddMetricsApiRequest request)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/hdd/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<AllHddMetricsApiResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public AllNetworkMetricsApiResponse GetNetworkMetrics(GetAllNetworkMetricsApiRequest request)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/network/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<AllNetworkMetricsApiResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public AllRamMetricsApiResponse GetRamMetrics(GetAllRamMetricsApiRequest request)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"{request.ClientBaseAddress}/api/metrics/ram/from/{request.FromTime}/to/{request.ToTime}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                return JsonConvert.DeserializeObject<AllRamMetricsApiResponse>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
