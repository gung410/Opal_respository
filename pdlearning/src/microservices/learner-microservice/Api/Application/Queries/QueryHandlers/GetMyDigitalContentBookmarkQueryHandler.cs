using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyDigitalContentBookmarkQueryHandler : BaseQueryHandler<GetMyDigitalContentBookmarkQuery, PagedResultDto<DigitalContentModel>>
    {
        private readonly IReadUserReviewShared _readUserReviewShared;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;
        private readonly IRepository<MyDigitalContent> _myDigitalContentRepository;

        public GetMyDigitalContentBookmarkQueryHandler(
            IUserContext userContext,
            IReadUserReviewShared readUserReviewShared,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<MyDigitalContent> myDigitalContentRepository) : base(userContext)
        {
            _readUserReviewShared = readUserReviewShared;
            _userBookmarkRepository = userBookmarkRepository;
            _myDigitalContentRepository = myDigitalContentRepository;
        }

        protected override async Task<PagedResultDto<DigitalContentModel>> HandleAsync(GetMyDigitalContentBookmarkQuery query, CancellationToken cancellationToken)
        {
            var bookmarkQuery = _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == BookmarkType.DigitalContent);

            var totalCount = await bookmarkQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(UserBookmark.CreatedDate)} DESC";
            bookmarkQuery = ApplySorting(bookmarkQuery, query.PageInfo, sortCriteria);
            bookmarkQuery = ApplyPaging(bookmarkQuery, query.PageInfo);

            var userBookmarks = await bookmarkQuery.ToListAsync(cancellationToken);

            var digitalContentIds = userBookmarks.Select(p => p.ItemId).ToList();

            var myDigitalContents = await _myDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => digitalContentIds.Contains(p.DigitalContentId))
                .ToListAsync(cancellationToken);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(digitalContentIds);

            var items = userBookmarks.Select(bookmark =>
            {
                var myDigitalContent =
                    myDigitalContents.FirstOrDefault(p => p.DigitalContentId == bookmark.ItemId);

                return DigitalContentModel.New(
                        bookmark.ItemId,
                        userReviewSummary[bookmark.ItemId].AverageRating,
                        userReviewSummary[bookmark.ItemId].ReviewCount)
                    .WithBookmarkInfo(bookmark)
                    .WithMyDigitalContent(myDigitalContent);
            }).ToList();

            return new PagedResultDto<DigitalContentModel>(totalCount, items);
        }
    }
}
