using System;
using System.Collections.Generic;

namespace MetricsManager.DAL
{
    public interface IMetricsRepository<T> : IRepository<T> where T : class
    {
        IList<T> GetByTimePeriod(TimeSpan from, TimeSpan to);

        IList<T> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to);

        IList<T> GetAllByAgentId(int agentId);

        TimeSpan GetMaxTimeByAgentId(int agentId);
    }
}
