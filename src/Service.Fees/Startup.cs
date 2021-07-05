using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJetWallet.Sdk.GrpcMetrics;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.DataReader;
using Prometheus;
using ProtoBuf.Grpc.Server;
using Service.Fees.Grpc;
using Service.Fees.Modules;
using Service.Fees.Services;
using Service.Fees.Settings;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;
using SimpleTrading.SettingsReader;

namespace Service.Fees
{
    public class Startup
    {
        private readonly MyNoSqlTcpClient _myNoSqlClient;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _myNoSqlClient = new MyNoSqlTcpClient(
                () => SettingsReader.ReadSettings<SettingsModel>(Program.SettingsFileName).MyNoSqlReaderHostPort,
                ApplicationEnvironment.HostName ??
                $"{ApplicationEnvironment.AppName}:{ApplicationEnvironment.AppVersion}");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(options =>
            {
                options.Interceptors.Add<PrometheusMetricsInterceptor>();
                options.BindMetricsInterceptors();
            });

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddMyTelemetry("SP-", Program.Settings.ZipkinUrl);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapGrpcSchema<FeesSettingsService, IFeesSettingsService>();
                endpoints.MapGrpcSchema<AssetFeesService, IAssetFeesService>();
                endpoints.MapGrpcSchema<SpotInstrumentFeesService, ISpotInstrumentFeesService>();
                endpoints.MapGrpcSchema<AssetFeesSettingsService, IAssetFeesSettingsService>();
                endpoints.MapGrpcSchema<SpotInstrumentFeesSettingsService, ISpotInstrumentFeesSettingsService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule(new ClientsModule(_myNoSqlClient));
            builder.RegisterModule(new MyNoSqlModule(() => GetSettings().MyNoSqlWriterUrl));
        }

        private SettingsModel GetSettings()
        {
            return SettingsReader.ReadSettings<SettingsModel>(Program.SettingsFileName);
        }
    }
}