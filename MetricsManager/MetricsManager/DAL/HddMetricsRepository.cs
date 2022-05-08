using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface IHddMetricsRepository : IMetricsRepository<HddMetric>
    {
    }

    public class HddMetricsRepository : IHddMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public HddMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(HddMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO hddmetrics(agentId, value, time) VALUES(@agentId, @value, @time)",
                new
                {
                    agentId = item.AgentId,
                    value = item.Value,
                    time = item.Time.TotalSeconds
                });
        }

        public void Delete(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("DELETE FROM hddmetrics WHERE id=@id", new { id });
        }

        public void Update(HddMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE hddmetrics SET value = @value, time = @time WHERE id = @id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
        }

        public IList<HddMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            // Читаем, используя Query, и в шаблон подставляем тип данных, объект которого Dapper, он сам заполнит его поля в соответствии с названиями колонок
            return connection.Query<HddMetric>("SELECT Id, agentId, Time, Value FROM hddmetrics").ToList();
        }

        public IList<HddMetric> GetAllByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<HddMetric>("SELECT Id, agentId, Time, Value FROM hddmetrics WHERE agentId = @agentId", new { agentId }).ToList();
        }

        public HddMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<HddMetric>("SELECT Id, agentId, Time, Value FROM hddmetrics WHERE id = @id", new { id });
        }

        public IList<HddMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<HddMetric>("SELECT Id, Time, Value FROM hddmetrics where time between @from and @to",
                new
                {
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public IList<HddMetric> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<HddMetric>("SELECT Id, Time, Value FROM hddmetrics where agentId = @agentId and time between @from and @to",
                new
                {
                    agentId = agentId,
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public TimeSpan GetMaxTimeByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<TimeSpan>("SELECT coalesce(max(Time), 0) FROM hddmetrics WHERE agentId = @agentId", new { agentId });
        }
    }
}
