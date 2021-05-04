using cxOrganization.Domain.AdvancedWorkContext;
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
            IAdvancedWorkContext workContext);

        public Task<BroadcastMessageDto> GetBroadcastMessageByIdAsync(
            int broadcastMessageId,
            IAdvancedWorkContext workContext);

        public Task<BroadcastMessageDto> DeleteBroadcastMessageAsync(
            int broadcastMessageId,
            IAdvancedWorkContext workContext);

        public Task<BroadcastMessageDto> CreateBroadcastMessageAsync(
            BroadcastMessageCreationDto broadcastMessage,
            IAdvancedWorkContext workContext);

        public Task<BroadcastMessageDto> UpdateBroadcastMessageAsync(
            BroadcastMessageDto broadcastMessage,
            IAdvancedWorkContext workContext);

        public Task<BroadcastMessageDto> ChangeBroadcastMessageStatusAsync(
            BroadcastMessageChangeStatusDto broadcastMessageChangeStatusDto,
            IAdvancedWorkContext workContext);

        public void SendScheduledBroadcastMessages();
    }
}
