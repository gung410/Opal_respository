using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Analytics.Application.Consumers.SAM.Messages;
using Microservice.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Analytics.Application.Consumers.SAM
{
    [OpalConsumer("cxid.system_success.sendcode.user")]
    public class UserSendCodeConsumer : ScopedOpalMessageConsumer<UserSendCodeMessage>
    {
        private readonly ILogger<UserSendCodeConsumer> _logger;
        private readonly IRepository<SAM_UserOtpClaim> _userOtpClaimRepository;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;

        public UserSendCodeConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserOtpClaim> userOtpClaimRepository,
            IRepository<SAM_UserHistory> userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<UserSendCodeConsumer>();
            _userOtpClaimRepository = userOtpClaimRepository;
            _userHistoryRepository = userHistoryRepository;
        }

        public async Task InternalHandleAsync(UserSendCodeMessage message)
        {
            var latestHistory = (await _userHistoryRepository.GetAllListAsync(t => t.UserId == message.UserId))
                .OrderByDescending(t => t.No)
                .FirstOrDefault();

            await _userOtpClaimRepository.InsertAsync(new SAM_UserOtpClaim
            {
                UserId = message.UserId,
                Type = message.SelectedProvider,
                UserHistoryId = latestHistory?.Id,
                Departmentid = latestHistory?.DepartmentId,
                ClaimDate = Clock.Now
            });
        }
    }
}
