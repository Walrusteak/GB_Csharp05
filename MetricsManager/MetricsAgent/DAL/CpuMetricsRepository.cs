using MetricsAgent.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MetricsAgent.DAL
{
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
    {
    }

    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public void Create(CpuMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)";
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "DELETE FROM cpumetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(CpuMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id = @id; ";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public IList<CpuMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM cpumetrics";
            List<CpuMetric> returnList = new();
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }
            return returnList;
        }

        public CpuMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM cpumetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public IList<CpuMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM cpumetrics where time between @from and @to";
            //cmd.CommandText = "SELECT * FROM cpumetrics where time >= @from and time <= @to";
            cmd.Parameters.AddWithValue("@from", from.TotalSeconds);
            cmd.Parameters.AddWithValue("@to", to.TotalSeconds);
            List<CpuMetric> returnList = new();
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }
            return returnList;
        }
    }
}
