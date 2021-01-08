using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class ConvertSuccessDetectorService : BackgroundService
    {
        private const int DelayMinute = 1;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConvertSuccessDetectorService> _logger;
        private bool _applicationStarted = false;

        public ConvertSuccessDetectorService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<ConvertSuccessDetectorService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started", nameof(ConvertSuccessDetectorService));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(DelayMinute), stoppingToken);

                if (!_applicationStarted)
                {
                    continue;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var convertingTrackingService = scope.ServiceProvider.GetService<IConvertingTrackingService>();

                    var convertingTrackings = await convertingTrackingService.GetConvertingTrackingsByStatus(ConvertStatus.Converting);

                    if (convertingTrackings.Any())
                    {
                        await convertingTrackingService.TrackingRecordingConvertSuccess(convertingTrackings);
                    }
                }
            }
        }
    }
}
