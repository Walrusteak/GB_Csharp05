using MetricsManager.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace MetricsManager.DAL
{
    public interface IAgentsRepository : IRepository<Agent>
    {
        public Agent GetByUrl(string url);
        public void Update(int id, bool enabled);
    }

    public class AgentsRepository : IAgentsRepository
    {
        private const string ConnectionString = "DataSource=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        public AgentsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(Agent item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("INSERT INTO agents(url, enabled) VALUES(@url, @enabled)",
                new
                {
                    url = item.Url,
                    enabled = item.Enabled
                });
        }

        public void Delete(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("DELETE FROM agents WHERE id=@id", new { id });
        }

        public void Update(Agent item)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE agents SET url = @url, enabled = @enabled WHERE id = @id",
                new
                {
                    id = item.Id,
                    url = item.Url,
                    enabled = item.Enabled
                });
        }

        public void Update(int id, bool enabled)
        {
            using SQLiteConnection connection = new(ConnectionString);
            connection.Execute("UPDATE agents SET enabled = @enabled WHERE id = @id", new { id, enabled });
        }

        public IList<Agent> GetAll()
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.Query<Agent>("SELECT Id, Url, Enabled FROM agents").ToList();
        }

        public Agent GetById(int id)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingle<Agent>("SELECT Id, Url, Enabled FROM agents WHERE id = @id", new { id });
        }

        public Agent GetByUrl(string url)
        {
            using SQLiteConnection connection = new(ConnectionString);
            return connection.QuerySingleOrDefault<Agent>("SELECT Id, Url, Enabled FROM agents WHERE url = @url", new { url });
        }
    }
}
