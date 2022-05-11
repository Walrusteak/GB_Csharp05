using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface INetworkMetricsRepository : IMetricsRepository<NetworkMetric>
    {
    }

    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public NetworkMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(NetworkMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO networkmetrics(agentId, value, time) VALUES(@agentId, @value, @time)",
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
            connection.Execute("DELETE FROM networkmetrics WHERE id=@id", new { id });
        }

        public void Update(NetworkMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE networkmetrics SET value = @value, time = @time WHERE id = @id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
        }

        public IList<NetworkMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            // Читаем, используя Query, и в шаблон подставляем тип данных, объект которого Dapper, он сам заполнит его поля в соответствии с названиями колонок
            return connection.Query<NetworkMetric>("SELECT Id, agentId, Time, Value FROM networkmetrics").ToList();
        }

        public IList<NetworkMetric> GetAllByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<NetworkMetric>("SELECT Id, agentId, Time, Value FROM networkmetrics WHERE agentId = @agentId", new { agentId }).ToList();
        }

        public NetworkMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<NetworkMetric>("SELECT Id, agentId, Time, Value FROM networkmetrics WHERE id = @id", new { id });
        }

        public IList<NetworkMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics where time between @from and @to",
                new
                {
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public IList<NetworkMetric> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<NetworkMetric>("SELECT Id, Time, Value FROM networkmetrics where agentId = @agentId and time between @from and @to",
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
            return connection.QuerySingle<TimeSpan>("SELECT coalesce(max(Time), 0) FROM networkmetrics WHERE agentId = @agentId", new { agentId });
        }
    }
}
