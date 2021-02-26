using cxOrganization.Domain.Dtos.BroadcastMessage;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface IBroadcastMessageService
    {
        public Task<PaginatedList<BroadcastMessageDto>> GetBroadcastMessagesAsync(
            BroadcastMessageSearchRequest broadcastMessageRequest,
            IWorkContext workContext,
            string token = null);

        public Task<BroadcastMessageDto> GetBroadcastMessageByIdAsync(
            int broadcastMessageId,
            IWorkContext workContext,
            string token = null);

        public Task<BroadcastMessageDto> DeleteBroadcastMessageAsync(
            int broadcastMessageId,
            IWorkContext workContext,
            string token = null);

        public Task<BroadcastMessageDto> CreateBroadcastMessageAsync(
            BroadcastMessageCreationDto broadcastMessage,
            IWorkContext workContext,
            string token = null);

        public Task<BroadcastMessageDto> UpdateBroadcastMessageAsync(
            BroadcastMessageDto broadcastMessage,
            IWorkContext workContext,
            string token = null);

        public Task<BroadcastMessageDto> ChangeBroadcastMessageStatusAsync(
            BroadcastMessageChangeStatusDto broadcastMessageChangeStatusDto,
            IWorkContext workContext,
            string token = null);

        public void SendScheduledBroadcastMessages();
    }
}
