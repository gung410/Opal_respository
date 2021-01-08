using cxPlatform.Client.ConexusBase;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public class SuspendOrDeactiveUserBackgroundJob : ISuspendOrDeactiveUserBackgroundJob
    {
        private readonly ILogger<SuspendOrDeactiveUserBackgroundJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SuspendOrDeactiveUserBackgroundJob(
            ILogger<SuspendOrDeactiveUserBackgroundJob> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task SuspendUserStatus()
        {
            _logger.LogInformation("START SUSPENDING USERS");

             var suspendUserService = new SuspendUserStatusStrategy(_logger, _serviceProvider);
            await suspendUserService.UpdateUserStatus();

            _logger.LogInformation("END SUSPENDING USERS");
        }

        public async Task DeActiveUserStatus()
        {
            _logger.LogInformation("START DEACTIVATING USERS");
            var deActiveUserService = new DeActiveUserStatusStrategy(_logger, _serviceProvider);
            await deActiveUserService.UpdateUserStatus();

            _logger.LogInformation("END DEACTIVATING USERS");
        }
    }
}
 