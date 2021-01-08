using System;
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
    [OpalConsumer("cxid.system_success.login.user")]
    public class UserLoginConsumer : ScopedOpalMessageConsumer<UserLoginMessage>
    {
        private readonly ILogger<UserLoginConsumer> _logger;
        private readonly IRepository<SAM_UserLogin> _userLoginRepository;
        private readonly IRepository<SAM_UserHistory> _userHistoryRepository;

        public UserLoginConsumer(
            ILoggerFactory loggerFactory,
            IRepository<SAM_UserLogin> userLoginRepository,
            IRepository<SAM_UserHistory> userHistoryRepository)
        {
            _logger = loggerFactory.CreateLogger<UserLoginConsumer>();
            _userLoginRepository = userLoginRepository;
            _userHistoryRepository = userHistoryRepository;
        }

        public async Task InternalHandleAsync(UserLoginMessage message)
        {
            var latestHistory = (await _userHistoryRepository.GetAllListAsync(t => t.UserId == message.UserId))
                .OrderByDescending(t => t.No)
                .FirstOrDefault();

            await _userLoginRepository.InsertAsync(new SAM_UserLogin()
            {
                UserId = message.UserId,
                LoginDate = Clock.Now,
                UserHistoryId = latestHistory?.Id,
                Type = message.LoginFromMobile ? "app" : "web",
                DepartmentId = latestHistory?.DepartmentId,
                ClientId = message.ClientId,
                SourceIp = message.SourceIp
            });
        }
    }
}
