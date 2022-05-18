using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface ICpuMetricsRepository : IMetricsRepository<CpuMetric>
    {
    }

    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public CpuMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(CpuMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO cpumetrics(agentId, value, time) VALUES(@agentId, @value, @time)",
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
            connection.Execute("DELETE FROM cpumetrics WHERE id=@id", new { id });
        }

        public void Update(CpuMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE cpumetrics SET value = @value, time = @time WHERE id = @id",
                new
                {
                    value = item.Value,
                    time = item.Time.TotalSeconds,
                    id = item.Id
                });
        }

        public IList<CpuMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            // Читаем, используя Query, и в шаблон подставляем тип данных, объект которого Dapper, он сам заполнит его поля в соответствии с названиями колонок
            return connection.Query<CpuMetric>("SELECT Id, agentId, Time, Value FROM cpumetrics").ToList();
        }

        public IList<CpuMetric> GetAllByAgentId(int agentId)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<CpuMetric>("SELECT Id, agentId, Time, Value FROM cpumetrics WHERE agentId = @agentId", new { agentId }).ToList();
        }

        public CpuMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<CpuMetric>("SELECT Id, agentId, Time, Value FROM cpumetrics WHERE id = @id", new { id });
        }

        public IList<CpuMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics where time between @from and @to",
                new
                {
                    from = from.TotalSeconds,
                    to = to.TotalSeconds
                }).ToList();
        }

        public IList<CpuMetric> GetByTimePeriod(int agentId, TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics where agentId = @agentId and time between @from and @to",
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
            return connection.QuerySingle<TimeSpan>("SELECT coalesce(max(Time), 0) FROM cpumetrics WHERE agentId = @agentId", new { agentId });
        }
    }
}
