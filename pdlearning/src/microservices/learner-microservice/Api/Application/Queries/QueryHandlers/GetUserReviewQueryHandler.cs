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
    public class GetUserReviewQueryHandler : BaseQueryHandler<GetUserReviewQuery, UserReviewModel>
    {
        private readonly IRepository<UserReview> _userReviewRepository;

        public GetUserReviewQueryHandler(
            IRepository<UserReview> userReviewRepository,
            IUserContext userContext) : base(userContext)
        {
            _userReviewRepository = userReviewRepository;
        }

        protected override async Task<UserReviewModel> HandleAsync(GetUserReviewQuery query, CancellationToken cancellationToken)
        {
            var userReview = await _userReviewRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == query.ItemId)
                .Where(p => p.ItemType == query.ItemType)
                .FirstOrDefaultAsync(cancellationToken);

            if (userReview == null)
            {
                return null;
            }

            return new UserReviewModel(userReview);
        }
    }
}
