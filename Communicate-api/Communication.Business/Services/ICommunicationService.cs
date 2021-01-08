using Communication.Business.Models;
using Communication.DataAccess.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Communication.Business.Services
{
    public interface ICommunicationService
    {
        Task SubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens);
        Task SendCommunicationAsync(CommunicationModelBase communicationModel);
        Task UnSubscribeAsync(ISet<string> topics, ISet<string> instanceIdtokens);
        Task SyncSubscribeAsync(ISet<string> topics, ISet<string> userids);
        Task<dynamic> RegisterCommunication(string userid, string deviceId, string platform, string instanceIdToken, string clientId);
        Task<bool> SetCommunicationRead(string notiticationid, string userId);
        Task<(int, int, int, IEnumerable<dynamic>)> GetCommunicationHistory(string userId, int pageNo,
            int pageSize, NotificationType? notificationType = null,
            DateTime? startDate = null, DateTime? endDate = null,
            DateTime? validOn = null, bool? getUnreadMessage = null,
            bool? getActionMessage = null);
        Task DeleteRegisterInfo(string registrationToken);
        Task TrackNotificationPullHistory(string userId);
        Task CancelNotification(string userId, string itemId);
        Task<Dictionary<string, List<NotificationDigestModel>>> GetDigestNotification(DateTime startDate, DateTime endDate);
        Task<bool> DeleteNotificationHistory(string notificationhistoryid);
        Task<bool> DeactiveNotification(string notificationId, string clientId);
        Task<List<Notification>> GetAllNotification(string clientId);
        Task<bool> UpdateNotification(string externalId, string clientId, NotificationUpdateModel notification);
    }
}