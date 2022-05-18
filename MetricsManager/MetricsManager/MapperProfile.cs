using AutoMapper;
using MetricsManager.Models;
using MetricsManager.Responses;

namespace MetricsManager
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Agent, AgentDto>();
            CreateMap<CpuMetric, CpuMetricDto>();
            CreateMap<DotNetMetric, DotNetMetricDto>();
            CreateMap<HddMetric, HddMetricDto>();
            CreateMap<NetworkMetric, NetworkMetricDto>();
            CreateMap<RamMetric, RamMetricDto>();

            CreateMap<AgentDto, Agent>();
            CreateMap<CpuMetricDto, CpuMetric>();
            CreateMap<DotNetMetricDto, DotNetMetric>();
            CreateMap<HddMetricDto, HddMetric>();
            CreateMap<NetworkMetricDto, NetworkMetric>();
            CreateMap<RamMetricDto, RamMetric>();
        }
    }
}
