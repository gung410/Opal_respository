using cxOrganization.Domain.Dtos.BroadcastMessage;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    public interface IBroadcastMessageService
    {
        public Task<PaginatedList<BroadcastMessageDto>> GetBroadcastMessagesAsync(
            BroadcastMessageSearchRequest broadcastMessageRequest);

        public Task<BroadcastMessageDto> GetBroadcastMessageByIdAsync(int broadcastMessageId);

        public Task<BroadcastMessageDto> DeleteBroadcastMessageAsync(int broadcastMessageId);

        public Task<BroadcastMessageDto> CreateBroadcastMessageAsync(BroadcastMessageCreationDto broadcastMessage, IWorkContext workContext);

        public Task<BroadcastMessageDto> UpdateBroadcastMessageAsync(BroadcastMessageDto broadcastMessage, IWorkContext workContext);

        public Task<BroadcastMessageDto> ChangeBroadcastMessageStatusAsync(BroadcastMessageChangeStatusDto broadcastMessageChangeStatusDto, IWorkContext workContext);

        public void SendScheduledBroadcastMessages();
    }
}
