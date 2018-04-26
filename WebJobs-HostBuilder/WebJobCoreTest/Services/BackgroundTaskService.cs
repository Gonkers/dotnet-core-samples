using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebJobCoreTest.Services
{
    class BackgroundTaskService : IHostedService
    {
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Task _backgroundTask;
        private readonly ILogger _logger;

        public BackgroundTaskService(ILogger<BackgroundTaskService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(BackgroundTaskService)} is starting.");
            _backgroundTask = Task.Run(BackgroundTask);
            _logger.LogDebug($"{nameof(BackgroundTaskService)} has started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(BackgroundTaskService)} is stopping.");
            _shutdown.Cancel();
            return Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task BackgroundTask()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                _logger.LogInformation($"Hello from {nameof(BackgroundTaskService)}!");
                await Task.Delay(4500);
            }
        }
    }
}