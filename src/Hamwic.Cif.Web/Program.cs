using Hamwic.Cif.Core.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using ILogger = Serilog.ILogger;

namespace Hamwic.Cif.Web
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = CreateLogger(Configuration);

            try
            {
                Log.Information("Building webhost {CurrentDirectory}", Directory.GetCurrentDirectory());
                var webHost = BuildWebHost(args);

                Log.Information("Running webhost");
                using (var scope = webHost.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        DbInitialiser.Initialise(context, services, Configuration["DefaultPassword"]).Wait();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the database.");
                    }
                }

                webHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        { 
        
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .UseDefaultServiceProvider(options => 
                    options.ValidateScopes = false)
                .Build();
        }

        /// <summary>
        /// Set up the Serilog logging
        /// </summary>
        /// <param name="config">Configuration settings</param>
        /// <returns></returns>
        private static ILogger CreateLogger(IConfiguration config)
        {
            var env = string.IsNullOrEmpty(config["Environment"]) ? "local" : config["Environment"];

            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "Hamwic.Cif.Web")
                .Enrich.WithProperty("env", env)
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(
                        "https://logsene-receiver.eu.sematext.com/75c9ba92-9687-4c67-bcac-38500a9ee817/hamwic.cif/"))
                    {
                        BatchPostingLimit = 2,
                        InlineFields = true,
                        MinimumLogEventLevel = LogEventLevel.Information
                    })
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error);

            if (!string.Equals(env, "prod", StringComparison.OrdinalIgnoreCase))
            {
                return loggerConfig
                    .WriteTo.RollingFile("./Logs/Hamwic.Cif.Web-{Date}.log")
                    .CreateLogger();
            }

            return loggerConfig.CreateLogger();
        }
    }
}
