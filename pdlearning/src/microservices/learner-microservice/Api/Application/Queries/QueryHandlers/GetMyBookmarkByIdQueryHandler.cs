using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyBookmarkByIdQueryHandler : BaseQueryHandler<GetMyBookmarkByIdQuery, UserBookmarkModel>
    {
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetMyBookmarkByIdQueryHandler(
            IRepository<UserBookmark> userBookmarkRepository,
            IUserContext userContext) : base(userContext)
        {
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override async Task<UserBookmarkModel> HandleAsync(GetMyBookmarkByIdQuery query, CancellationToken cancellationToken)
        {
            var userBookmark = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.Id == query.Id)
                .Where(p => p.UserId == CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (userBookmark == null)
            {
                return null;
            }

            return new UserBookmarkModel(userBookmark);
        }
    }
}
