using AutoMapper;
using FluentMigrator.Runner;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsManager.Jobs;
using MetricsManager.Jobs.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace MetricsManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string ConnectionString = @"Data Source=metrics.db;Version=3;";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureSqlLiteConnection(services);
            services.AddSingleton<IAgentsRepository, AgentsRepository>();
            services.AddSingleton<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddSingleton<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddSingleton<IHddMetricsRepository, HddMetricsRepository>();
            services.AddSingleton<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddSingleton<IRamMetricsRepository, RamMetricsRepository>();

            MapperConfiguration mapperConfiguration = new(mp => mp.AddProfile(new MapperProfile()));
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(ConnectionString)
                    .ScanIn(typeof(Startup).Assembly).For.Migrations()
                ).AddLogging(lb => lb.AddFluentMigratorConsole());

            services.AddMvc().AddNewtonsoftJson();  //меняем стандартный парсер во имя таймспанов

            services.AddHttpClient<IMetricsAgentClient, MetricsAgentClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));

            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<CpuMetricsJob>();
            services.AddSingleton<DotNetMetricsJob>();
            services.AddSingleton<HddMetricsJob>();
            services.AddSingleton<NetworkMetricsJob>();
            services.AddSingleton<RamMetricsJob>();
            services.AddSingleton(new JobSchedule(typeof(CpuMetricsJob), "0/5 * * * * ?")); // Запускать каждые 5 секунд
            services.AddSingleton(new JobSchedule(typeof(DotNetMetricsJob), "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(HddMetricsJob), "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(NetworkMetricsJob), "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(RamMetricsJob), "0/5 * * * * ?"));
            services.AddHostedService<QuartzHostedService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API сервиса агента сбора метрик",
                    Description = "Здесь можно поиграть с api нашего сервиса",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Yagello V",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/Walrusteak"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Можно указать, под какой лицензией всё опубликовано",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureSqlLiteConnection(IServiceCollection services)
        {
            const string connectionString = "DataSource = metrics.db; Version = 3; Pooling = true; Max Pool Size = 100; ";
            SQLiteConnection connection = new(connectionString);
            connection.Open();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API сервиса менеджера сбора метрик");
                c.RoutePrefix = string.Empty;
            });

            migrationRunner.MigrateUp();
        }
    }
}
