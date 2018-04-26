using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebJobCoreTest.Services;

namespace WebJobCoreTest
{
    public class Program
    {
        IConfiguration Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            using (var iAmTheWatcherOnTheWalls = new WebJobsShutdownWatcher())
            {
                var hostBuilder = new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                        if (args != null && args.Length > 0)
                            config.AddCommandLine(args);
                    })
                    .ConfigureServices((hostingContext, services) =>
                    {
                        services.AddOptions();
                        services.Configure<AppSettings>(hostingContext.Configuration);
                        services.AddSingleton<IHostedService, ExampleTimerService>();
                        services.AddSingleton<IHostedService, BackgroundTaskService>();
                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    });

                await hostBuilder.RunConsoleAsync(iAmTheWatcherOnTheWalls.Token);
            }
        }
    }
}