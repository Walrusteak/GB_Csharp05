using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class NetworkMetricJob : IJob
	{
		private INetworkMetricsRepository _repository;
		private PerformanceCounter _networkCounter;

		public NetworkMetricJob(INetworkMetricsRepository repository)
		{
			_repository = repository;
			_networkCounter = new("Network Adapter", "Bytes Total/sec", "Сетевой адаптер Broadcom 802.11n _2");
		}

		public Task Execute(IJobExecutionContext context)
		{
			int value = Convert.ToInt32(_networkCounter.NextValue());
			TimeSpan time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
			_repository.Create(new Models.NetworkMetric { Time = time, Value = value });
			return Task.CompletedTask;
		}
	}
}