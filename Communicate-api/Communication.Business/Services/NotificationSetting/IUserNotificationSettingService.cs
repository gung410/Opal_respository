using System.Threading.Tasks;

using Communication.Business.Models.UserNotificationSetting;
using Communication.DataAccess.Notification;
using CSharpFunctionalExtensions;

namespace Communication.Business.Services.NotificationSetting
{
    public interface IUserNotificationSettingService
    {
        Task<Result> RegisterUserNotificationSetting(UserNotificationSettingCommand command);
        Task<Result<UserNotificationSetting>> GetUserNotificationSetting(string userId);
    }
}
