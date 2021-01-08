using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyDigitalContentByIdsQueryHandler : BaseQueryHandler<GetMyDigitalContentByIdsQuery, List<DigitalContentModel>>
    {
        private readonly IReadUserReviewShared _readUserReviewShared;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;

        public GetMyDigitalContentByIdsQueryHandler(
            IUserContext userContext,
            IReadUserReviewShared readUserReviewShared,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<MyDigitalContent> myDigitalContentRepository) : base(userContext)
        {
            _readUserReviewShared = readUserReviewShared;
            _userBookmarkRepository = userBookmarkRepository;
            _myDigitalContentRepository = myDigitalContentRepository;
        }

        protected override async Task<List<DigitalContentModel>> HandleAsync(GetMyDigitalContentByIdsQuery query, CancellationToken cancellationToken)
        {
            var digitalContentIds = query.DigitalContentIds.Distinct().ToList();

            var myDigitalContents = await _myDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => digitalContentIds.Contains(p.DigitalContentId))
                .ToListAsync(cancellationToken);

            var bookmarks = await _userBookmarkRepository.GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == BookmarkType.DigitalContent)
                .Where(p => digitalContentIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(digitalContentIds);

            return query.DigitalContentIds.Select(digitalContentId =>
            {
                var bookmark = bookmarks.FirstOrDefault(p => p.ItemId == digitalContentId);
                var myDigitalContent = myDigitalContents.FirstOrDefault(p => p.DigitalContentId == digitalContentId);

                return DigitalContentModel.New(
                        digitalContentId,
                        userReviewSummary[digitalContentId].AverageRating,
                        userReviewSummary[digitalContentId].ReviewCount)
                    .WithBookmarkInfo(bookmark)
                    .WithMyDigitalContent(myDigitalContent);
            }).ToList();
        }
    }
}
