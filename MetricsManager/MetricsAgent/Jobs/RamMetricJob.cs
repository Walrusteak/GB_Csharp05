using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class RamMetricJob : IJob
	{
		private IRamMetricsRepository _repository;
		private PerformanceCounter _ramCounter;

		public RamMetricJob(IRamMetricsRepository repository)
		{
			_repository = repository;
			_ramCounter = new("Memory", "Available MBytes");
		}

		public Task Execute(IJobExecutionContext context)
		{
			int value = Convert.ToInt32(_ramCounter.NextValue());
			TimeSpan time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
			_repository.Create(new Models.RamMetric { Time = time, Value = value });
			return Task.CompletedTask;
		}
	}
}