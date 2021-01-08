using System.Threading.Tasks;
using LearnerApp.Models.Communication;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ICommunicationBackendService
    {
        [Get("/communication/notification/get_notification_history?userId={userId}")]
        Task<CommunicationResult<Notification>> GetNotificationHistory(string userId);
    }
}
