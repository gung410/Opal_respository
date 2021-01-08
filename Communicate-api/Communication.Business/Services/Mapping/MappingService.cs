using Communication.Business;
using Communication.Business.Models;
using System;

namespace Communication.Business.Services.Mapping
{
    public class MappingService : IMappingService
    {
        public DataAccess.Notification.Notification ToNotificationEntity(PushNotificationModel model)
        {
            return new Communication.DataAccess.Notification.Notification
            {
                Active = true,
                ChangedBy = "",
                ChangedDateUtc = DateTime.UtcNow,
                CreatedBy = "",
                CreatedDateUtc = DateTime.UtcNow,
            };
        }
    }
}
