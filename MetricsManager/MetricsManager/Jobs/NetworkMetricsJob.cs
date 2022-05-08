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
	public class NetworkMetricsJob : IJob
	{
		private readonly IAgentsRepository _agentsRepository;
		private readonly INetworkMetricsRepository _repository;
		private readonly IMetricsAgentClient _client;
		private readonly IMapper _mapper;

		public NetworkMetricsJob(IAgentsRepository agents, INetworkMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
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
				GetAllNetworkMetricsApiRequest request = new()
				{
					FromTime = _repository.GetMaxTimeByAgentId(agent.Id),
					ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
					ClientBaseAddress = agent.Url
				};
				AllNetworkMetricsApiResponse response = _client.GetNetworkMetrics(request);
				foreach (NetworkMetricDto metric in response.Metrics)
				{
					metric.AgentId = agent.Id;
					_repository.Create(_mapper.Map<NetworkMetric>(metric));
				}
			}
			return Task.CompletedTask;
		}
	}
}
