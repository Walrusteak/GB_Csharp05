using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class HddMetricJob : IJob
	{
		private IHddMetricsRepository _repository;
		private PerformanceCounter _hddCounter;

		public HddMetricJob(IHddMetricsRepository repository)
		{
			_repository = repository;
			_hddCounter = new("LogicalDisk", "% Free Space", "C:");
		}

		public Task Execute(IJobExecutionContext context)
		{
			int value = Convert.ToInt32(_hddCounter.NextValue());
			TimeSpan time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
			_repository.Create(new Models.HddMetric { Time = time, Value = value });
			return Task.CompletedTask;
		}
	}
}