using MetricsAgent.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MetricsAgent.DAL
{
    public interface IRamMetricsRepository : IRepository<RamMetric>
    {
    }

    public class RamMetricsRepository : IRamMetricsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public void Create(RamMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "INSERT INTO rammetrics(value, time) VALUES(@value, @time)";
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
            cmd.CommandText = "DELETE FROM rammetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public void Update(RamMetric item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "UPDATE rammetrics SET value = @value, time = @time WHERE id = @id; ";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public IList<RamMetric> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM rammetrics";
            List<RamMetric> returnList = new();
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new RamMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }
            return returnList;
        }

        public RamMetric GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM rammetrics WHERE id=@id";
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new RamMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(1))
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public IList<RamMetric> GetByTimePeriod(TimeSpan from, TimeSpan to)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Open();
            using SQLiteCommand cmd = new(connection);
            cmd.CommandText = "SELECT * FROM rammetrics where time between @from and @to";
            List<RamMetric> returnList = new();
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new RamMetric
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
