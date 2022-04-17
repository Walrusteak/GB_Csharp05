using MetricsAgent.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsAgent.DAL
{
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
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
            connection.Execute("INSERT INTO cpumetrics(value, time) VALUES(@value, @time)",
                new
                {
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
            return connection.Query<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics").ToList();
        }

        public CpuMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics WHERE id = @id", new { id });
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
    }
}
