using AutoMapper;
using FluentMigrator.Runner;
using MetricsAgent.DAL;
using MetricsAgent.Jobs;
using MetricsAgent.Jobs.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace MetricsAgent
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

            services.AddMvc().AddNewtonsoftJson();  //������ ����������� ������ �� ��� ����������

            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<CpuMetricJob>();
            services.AddSingleton<DotNetMetricJob>();
            services.AddSingleton<HddMetricJob>();
            services.AddSingleton<NetworkMetricJob>();
            services.AddSingleton<RamMetricJob>();
            services.AddSingleton(new JobSchedule(typeof(CpuMetricJob),     "0/5 * * * * ?")); // ��������� ������ 5 ������
            services.AddSingleton(new JobSchedule(typeof(DotNetMetricJob),  "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(HddMetricJob),     "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(NetworkMetricJob), "0/5 * * * * ?"));
            services.AddSingleton(new JobSchedule(typeof(RamMetricJob),     "0/5 * * * * ?"));
            services.AddHostedService<QuartzHostedService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API ������� ������ ����� ������",
                    Description = "����� ����� �������� � api ������ �������",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Yagello V",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/Walrusteak"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "����� �������, ��� ����� ��������� �� ������������",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                // ��������� ����, �� �������� ����� ����� ����������� ��� Swagger UI
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

            // ��������� middleware � �������� ��� ��������� Swagger-��������.
            app.UseSwagger();
            // ��������� middleware ��� ��������� swagger-ui
            // ��������� �������� Swagger JSON (���� ���������� �� ��������������� �������������,
            // �� ������� ����� �������� UI).
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ������� ������ ����� ������");
                c.RoutePrefix = string.Empty;
            });

            migrationRunner.MigrateUp();
        }
    }
}
