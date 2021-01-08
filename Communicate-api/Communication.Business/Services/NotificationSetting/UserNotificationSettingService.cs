using System.Threading.Tasks;

using Communication.Business.Models.UserNotificationSetting;
using Communication.DataAccess.Notification;

using CSharpFunctionalExtensions;

using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Communication.Business.Services.NotificationSetting
{
    public class UserNotificationSettingService : IUserNotificationSettingService
    {
        private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
        private readonly ILogger<UserNotificationSettingService> _logger;

        public UserNotificationSettingService(IUserNotificationSettingRepository userNotificationSettingRepository,
            ILogger<UserNotificationSettingService> logger)
        {
            _userNotificationSettingRepository = userNotificationSettingRepository;
            _logger = logger;
        }
        public async Task<Result> RegisterUserNotificationSetting(UserNotificationSettingCommand command)
        {
            if (string.IsNullOrEmpty(command.UserId))
            {
                _logger.LogInformation("userid is not valid");
                return Result.Failure("userid is not valid");
            }
            var userSetting = await _userNotificationSettingRepository.GetSettingbyUserIdAsync(command.UserId);
            var isInsertNew = userSetting == null;
            if (isInsertNew)
            {
                userSetting = new UserNotificationSetting
                {
                    UserId = command.UserId
                };
            }

            userSetting.NotificationChannel = command.NotificationChannel;
            if (command.EnableDigest)
            {
                userSetting.EnableDigest = true;
                if (command.DigestEmailAt != null)
                    userSetting.DigestEmailAt = new DigestEmailAt
                    {
                        Hour = command.DigestEmailAt.Hour,
                        Minute = command.DigestEmailAt.Minute
                    };
            }
            if (isInsertNew)
                await _userNotificationSettingRepository.InsertOneAsync(userSetting);
            else
                await _userNotificationSettingRepository.Update(userSetting.Id,
                    new UpdateDefinitionBuilder<UserNotificationSetting>()
                .Set("NotificationChannel", userSetting.NotificationChannel)
                .Set("DigestEmailAt", userSetting.DigestEmailAt)
                .Set("EnableDigest", command.EnableDigest));

            return Result.Success();
        }
        public async Task<Result<UserNotificationSetting>> GetUserNotificationSetting(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Result.Failure<UserNotificationSetting>("userId is not valid");
            }
            var userSetting = await _userNotificationSettingRepository.GetSettingbyUserIdAsync(userId);
            if (userSetting == null)
            {
                userSetting = new UserNotificationSetting
                {
                    UserId = userId,
                    NotificationChannel = NotificationChannel.Email
                };
            }
            return Result.Success<UserNotificationSetting>(userSetting);
        }
    }
}
