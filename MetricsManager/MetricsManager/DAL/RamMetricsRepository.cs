using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface IRamMetricsRepository : IMetricsRepository<RamMetric>
    {
    }

    public class RamMetricsRepository : IRamMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public RamMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(RamMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO rammetrics(agentId, value, time) VALUES(@agentId, @value, @time)",
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
            connection.Execute("DELETE FROM rammetrics WHERE id=@id", new { id });
        }

        public void Update(RamMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE rammetrics SET value = @value, time = @time WHERE id = @id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
        }

        public IList<RamMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            // Читаем, используя Query, и в шаблон подставляем тип данных, объект которого Dapper, он сам заполнит его поля в соответствии с названиями колонок
            return connection.Query<RamMetric>("SELECT Id, agentId, Time, Value FROM rammetrics").ToList();
        }

        public IList<RamMetric> GetAllByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<RamMetric>("SELECT Id, agentId, Time, Value FROM rammetrics WHERE agentId = @agentId", new { agentId }).ToList();
        }

        public RamMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<RamMetric>("SELECT Id, agentId, Time, Value FROM rammetrics WHERE id = @id", new { id });
        }

        public IList<RamMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<RamMetric>("SELECT Id, Time, Value FROM rammetrics where time between @from and @to",
                new
                {
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public IList<RamMetric> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<RamMetric>("SELECT Id, Time, Value FROM rammetrics where agentId = @agentId and time between @from and @to",
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
            return connection.QuerySingle<TimeSpan>("SELECT coalesce(max(Time), 0) FROM rammetrics WHERE agentId = @agentId", new { agentId });
        }
    }
}
