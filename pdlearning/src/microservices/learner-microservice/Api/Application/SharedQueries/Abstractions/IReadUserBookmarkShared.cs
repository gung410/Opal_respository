using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.SharedQueries.Abstractions
{
    public interface IReadUserBookmarkShared : ISharedQuery
    {
        Task<UserBookmark> GetByItemId(Guid userId, Guid itemId);

        Task<List<UserBookmark>> GetByItemIds(Guid userId, List<Guid> itemIds);
    }
}
