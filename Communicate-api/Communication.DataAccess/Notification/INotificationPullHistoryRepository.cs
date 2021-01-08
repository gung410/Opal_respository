using System.Threading.Tasks;

namespace Communication.DataAccess.Notification
{
    public interface INotificationPullHistoryRepository : IRepository<NotificationPullHistory>
    {
        Task ReplaceOneAsync(NotificationPullHistory notificationPullHistory);
    }
}
