using cxPlatform.Core;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities.BroadcastMessage;

namespace cxOrganization.Domain.Repositories
{
    public interface IBroadcastMessageRepository : IRepository<BroadcastMessageEntity>
    {
        public IQueryable<BroadcastMessageEntity> GetBroadcastMessagesAsNoTracking();

        public Task<BroadcastMessageEntity> GetBroadcastMessageByIdAsync(int broadcastMessageId);

        public Task<BroadcastMessageEntity> UpdateBroadcastMessageAsync(BroadcastMessageEntity broadcastMessage);

        public Task<BroadcastMessageEntity> CreateBroadcastMessageAsync(BroadcastMessageEntity broadcastMessage);

        public Task<BroadcastMessageEntity> DeleteBroadcastMessageAsync(int broadcastMessageId);
    }
}
