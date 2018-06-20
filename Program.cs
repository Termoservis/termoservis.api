using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace termoservis.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "All the way to the bottom.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            .WriteTo.File("Logs/termoservisapi.txt", rollingInterval: RollingInterval.Day)
                            .WriteTo.Sentry("https://6b5044dc64264c0da89d22a02d9225af:1c9939f46a794ef8a867cb804af2c3c2@sentry.ops.dfnoise.com/4", restrictedToMinimumLevel: LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .Destructure.With<HttpContextDestructingPolicy>()
                            .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)
                            .CreateLogger();
            Log.Logger.Information("May the force be with you.");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
