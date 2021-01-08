using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microservice.WebinarAutoscaler.Application.HostedServices
{
    public class SeekCoordinatedBBBServerService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<SeekCoordinatedBBBServerService> _logger;

        private bool _applicationStarted;

        public SeekCoordinatedBBBServerService(
            IServiceProvider provider,
            IHostApplicationLifetime lifetime,
            ILogger<SeekCoordinatedBBBServerService> logger)
        {
            _provider = provider;
            _logger = logger;
            lifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            lifetime.ApplicationStopping.Register(() => _applicationStarted = false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Process();
        }

        private async Task Process()
        {
            while (true)
            {
                await Task.Delay(1000);

                if (!_applicationStarted)
                {
                    continue;
                }

                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var bbbSyncService = scope.ServiceProvider.GetService<IBBBSyncService>();
                        await bbbSyncService.SeekCoordinatedBBBServerAsync();
                    }
                }
                catch (System.Exception exception)
                {
                    _logger.LogError("Errors occurred: {Exception}", exception);
                }
            }
        }
    }
}
