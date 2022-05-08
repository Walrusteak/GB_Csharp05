using MetricsAgent.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsAgent.DAL
{
    public interface IDotNetMetricsRepository : IRepository<DotNetMetric>
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
            connection.Execute("INSERT INTO dotnetmetrics(value, time) VALUES(@value, @time)",
                new
                {
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
            return connection.Query<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics").ToList();
        }

        public DotNetMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<DotNetMetric>("SELECT Id, Time, Value FROM dotnetmetrics WHERE id = @id", new { id });
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
    }
}
