using MetricsAgent.DAL;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
	public class DotNetMetricJob : IJob
	{
		private IDotNetMetricsRepository _repository;
		private PerformanceCounter _dotNetCounter;

		public DotNetMetricJob(IDotNetMetricsRepository repository)
		{
			_repository = repository;
			_dotNetCounter = new(".NET CLR Memory", "Gen 0 heap size", "_Global_");
		}

		public Task Execute(IJobExecutionContext context)
		{
			int value = Convert.ToInt32(_dotNetCounter.NextValue());
			TimeSpan time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
			_repository.Create(new Models.DotNetMetric { Time = time, Value = value });
			return Task.CompletedTask;
		}
	}
}