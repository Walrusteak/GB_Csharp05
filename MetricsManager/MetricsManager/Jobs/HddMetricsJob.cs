using AutoMapper;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsManager.Models;
using MetricsManager.Requests;
using MetricsManager.Responses;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Jobs
{
	public class HddMetricsJob : IJob
	{
		private readonly IAgentsRepository _agentsRepository;
		private readonly IHddMetricsRepository _repository;
		private readonly IMetricsAgentClient _client;
		private readonly IMapper _mapper;

		public HddMetricsJob(IAgentsRepository agents, IHddMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
		{
			_agentsRepository = agents;
			_repository = repository;
			_client = client;
			_mapper = mapper;
		}

		public Task Execute(IJobExecutionContext context)
		{
			IList<Agent> agents = _agentsRepository.GetAll();
			foreach (Agent agent in agents.Where(a => a.Enabled))
			{
				GetAllHddMetricsApiRequest request = new()
				{
					FromTime = _repository.GetMaxTimeByAgentId(agent.Id),
					ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
					ClientBaseAddress = agent.Url
				};
				AllHddMetricsApiResponse response = _client.GetHddMetrics(request);
				foreach (HddMetricDto metric in response.Metrics)
				{
					metric.AgentId = agent.Id;
					_repository.Create(_mapper.Map<HddMetric>(metric));
				}
			}
			return Task.CompletedTask;
		}
	}
}
