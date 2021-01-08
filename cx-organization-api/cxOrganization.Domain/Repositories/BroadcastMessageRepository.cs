using System;
using cxPlatform.Core;
using System.Linq;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities.BroadcastMessage;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Repositories
{
    public class BroadcastMessageRepository : RepositoryBase<BroadcastMessageEntity>, IBroadcastMessageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastMessageRepository" /> class.
        /// </summary>
        /// <param name="dbContext">The unit of work.</param>

        private readonly OrganizationDbContext _organizationDbContext;

        public BroadcastMessageRepository(OrganizationDbContext dbContext) : base(dbContext)
        {
            _organizationDbContext = dbContext;
        }

        public IQueryable<BroadcastMessageEntity> GetBroadcastMessagesAsNoTracking()
        {
            return GetAllAsNoTracking().Where(x => x.Deleted == null);
        }

        public async Task<BroadcastMessageEntity> GetBroadcastMessageByIdAsync(int broadcastMessageId)
        {
            return await GetAll()
                .FirstOrDefaultAsync(x => x.Deleted == null && x.BroadcastMessageId == broadcastMessageId);
        }

        public async Task<BroadcastMessageEntity> UpdateBroadcastMessageAsync(BroadcastMessageEntity broadcastMessage)
        {
            broadcastMessage.LastUpdated = DateTime.UtcNow;
            var updatedEntity = Update(broadcastMessage);
            await _organizationDbContext.SaveChangesAsync();

            return updatedEntity;
        }

        public async Task<BroadcastMessageEntity> CreateBroadcastMessageAsync(BroadcastMessageEntity broadcastMessage)
        {
            // Update CreatedDate
            var currentTime = DateTime.UtcNow;
            broadcastMessage.CreatedDate = currentTime;
            broadcastMessage.LastUpdated = currentTime;

            var createdEntity = Insert(broadcastMessage);
            await _organizationDbContext.SaveChangesAsync();

            return createdEntity;
        }

        public async Task<BroadcastMessageEntity> DeleteBroadcastMessageAsync(int broadcastMessageId)
        {
            var deleteEntity = await GetByIdAsync(broadcastMessageId);

            deleteEntity.Deleted = DateTime.UtcNow;
            deleteEntity.LastUpdated = DateTime.UtcNow;

            Update(deleteEntity);

            await _organizationDbContext.SaveChangesAsync();

            return deleteEntity;
        }
    }
}
