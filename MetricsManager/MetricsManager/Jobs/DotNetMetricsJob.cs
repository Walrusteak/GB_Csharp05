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
	public class DotNetMetricsJob : IJob
	{
		private readonly IAgentsRepository _agentsRepository;
		private readonly IDotNetMetricsRepository _repository;
		private readonly IMetricsAgentClient _client;
		private readonly IMapper _mapper;

		public DotNetMetricsJob(IAgentsRepository agents, IDotNetMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
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
				GetAllDotNetMetricsApiRequest request = new()
				{
					FromTime = _repository.GetMaxTimeByAgentId(agent.Id),
					ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
					ClientBaseAddress = agent.Url
				};
				AllDotNetMetricsApiResponse response = _client.GetDotNetMetrics(request);
				foreach (DotNetMetricDto metric in response.Metrics)
				{
					metric.AgentId = agent.Id;
					_repository.Create(_mapper.Map<DotNetMetric>(metric));
				}
			}
			return Task.CompletedTask;
		}
	}
}
