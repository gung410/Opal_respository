using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.SharedQueries
{
    public class ReadUserBookmarkShared : BaseReadShared<UserBookmark>, IReadUserBookmarkShared
    {
        public ReadUserBookmarkShared(IReadOnlyRepository<UserBookmark> readUserBookmarkRepository) : base(readUserBookmarkRepository)
        {
        }

        public Task<UserBookmark> GetByItemId(Guid userId, Guid itemId)
        {
            var itemIds = new List<Guid> { itemId };

            return FilterByItemIdsQuery(userId, itemIds).FirstOrDefaultAsync();
        }

        public Task<List<UserBookmark>> GetByItemIds(Guid userId, List<Guid> itemIds)
        {
            if (!itemIds.Any())
            {
                return Task.FromResult(new List<UserBookmark>());
            }

            return FilterByItemIdsQuery(userId, itemIds).ToListAsync();
        }

        private IQueryable<UserBookmark> FilterByItemIdsQuery(Guid userId, List<Guid> itemIds)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => itemIds.Contains(p.ItemId));
        }
    }
}
