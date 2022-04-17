using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using MetricsAgent.DAL;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;


namespace MetricsAgent
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureSqlLiteConnection(services);
            services.AddScoped<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddScoped<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddScoped<IHddMetricsRepository, HddMetricsRepository>();
            services.AddScoped<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddScoped<IRamMetricsRepository, RamMetricsRepository>();

            MapperConfiguration mapperConfiguration = new(mp => mp.AddProfile(new MapperProfile()));
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc().AddNewtonsoftJson();  //меняем стандартный парсер во имя таймспанов
        }

        private void ConfigureSqlLiteConnection(IServiceCollection services)
        {
            const string connectionString = "DataSource = metrics.db; Version = 3; Pooling = true; Max Pool Size = 100; ";
            SQLiteConnection connection = new(connectionString);
            connection.Open();
            PrepareSchema(connection);
            PrepareData(connection);
        }

        private void PrepareSchema(SQLiteConnection connection)
        {
            using SQLiteCommand command = new(connection);
            CreateDefaultTable(command, "cpumetrics");
            CreateDefaultTable(command, "rammetrics");
            CreateDefaultTable(command, "hddmetrics");
            CreateDefaultTable(command, "networkmetrics");
            CreateDefaultTable(command, "dotnetmetrics");
        }

        private void CreateDefaultTable(SQLiteCommand cmd, string tableName)
        {
            cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"CREATE TABLE {tableName}(id INTEGER PRIMARY KEY, value INT, time INT)";
            cmd.ExecuteNonQuery();
        }

        private void PrepareData(SQLiteConnection connection)
        {
            using SQLiteCommand command = new(connection);
            InsertRandomMetrics(command, "cpumetrics", 10);
            InsertRandomMetrics(command, "rammetrics", 10);
            InsertRandomMetrics(command, "hddmetrics", 10);
            InsertRandomMetrics(command, "networkmetrics", 10);
            InsertRandomMetrics(command, "dotnetmetrics", 10);
        }

        private void InsertRandomMetrics(SQLiteCommand cmd, string tableName, int count)
        {
            Random random = new();
            for (int i = 1; i <= count; i++)
            {
                cmd.CommandText = $"INSERT INTO {tableName}(value, time) VALUES({random.Next(0, 100)}, {i})";
                cmd.ExecuteNonQuery();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
