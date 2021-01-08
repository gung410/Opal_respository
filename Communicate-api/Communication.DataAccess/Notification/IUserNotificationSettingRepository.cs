using System.Threading.Tasks;

namespace Communication.DataAccess.Notification
{
    public interface IUserNotificationSettingRepository : IRepository<UserNotificationSetting>
    {
        Task<UserNotificationSetting> GetSettingbyUserIdAsync(string userId);
    }
}
