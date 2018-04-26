using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebJobCoreTest.Services
{
    class ExampleTimerService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppSettings> _appConfig;
        private Timer _timer;

        public ExampleTimerService(ILogger<ExampleTimerService> logger, IOptions<AppSettings> appConfig)
        {
            _logger = logger;
            _appConfig = appConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ExampleTimerService)} is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation($"Background work with text: {_appConfig.Value.SomeSettingToPoopOn}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ExampleTimerService)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}