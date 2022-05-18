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
    public class CpuMetricsJob : IJob
	{
		private readonly IAgentsRepository _agentsRepository;
		private readonly ICpuMetricsRepository _repository;
		private readonly IMetricsAgentClient _client;
		private readonly IMapper _mapper;

		public CpuMetricsJob(IAgentsRepository agents, ICpuMetricsRepository repository, IMetricsAgentClient client, IMapper mapper)
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
				GetAllCpuMetricsApiRequest request = new()
				{
					FromTime = _repository.GetMaxTimeByAgentId(agent.Id),
					ToTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
					ClientBaseAddress = agent.Url
				};
                AllCpuMetricsApiResponse response = _client.GetCpuMetrics(request);
				foreach (CpuMetricDto metric in response.Metrics)
				{
					metric.AgentId = agent.Id;
					_repository.Create(_mapper.Map<CpuMetric>(metric));
				}
			}
			return Task.CompletedTask;
		}
	}
}
