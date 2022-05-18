using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface IDotNetMetricsRepository : IMetricsRepository<DotNetMetric>
    {
    }

    public class DotNetMetricsRepository : IDotNetMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public DotNetMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(DotNetMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO dotnetmetrics(agentId, value, time) VALUES(@agentId, @value, @time)",
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
            connection.Execute("DELETE FROM dotnetmetrics WHERE id=@id", new { id });
        }

        public void Update(DotNetMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE dotnetmetrics SET value = @value, time = @time WHERE id = @id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
        }

        public IList<DotNetMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            // Читаем, используя Query, и в шаблон подставляем тип данных, объект которого Dapper, он сам заполнит его поля в соответствии с названиями колонок
            return connection.Query<DotNetMetric>("SELECT Id, agentId, Time, Value FROM dotnetmetrics").ToList();
        }

        public IList<DotNetMetric> GetAllByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<DotNetMetric>("SELECT Id, agentId, Time, Value FROM dotnetmetrics WHERE agentId = @agentId", new { agentId }).ToList();
        }

        public DotNetMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<DotNetMetric>("SELECT Id, agentId, Time, Value FROM dotnetmetrics WHERE id = @id", new { id });
        }

        public IList<DotNetMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics where time between @from and @to",
                new
                {
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public IList<DotNetMetric> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics where agentId = @agentId and time between @from and @to",
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
            return connection.QuerySingle<TimeSpan>("SELECT coalesce(max(Time), 0) FROM dotnetmetrics WHERE agentId = @agentId", new { agentId });
        }
    }
}
