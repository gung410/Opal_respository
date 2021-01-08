using Communication.Business.Models;

namespace Communication.Business.Services.Mapping
{
    public interface IMappingService
    {
        DataAccess.Notification.Notification ToNotificationEntity(PushNotificationModel model);
    }
}