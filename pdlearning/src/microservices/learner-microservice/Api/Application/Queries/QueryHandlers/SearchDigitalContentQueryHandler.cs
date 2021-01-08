using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class SearchDigitalContentQueryHandler :
        BaseQueryHandler<SearchDigitalContentQuery, SearchPagedResultDto<DigitalContentModel, MyDigitalContentStatisticModel>>
    {
        private readonly IReadUserReviewShared _readUserReviewShared;
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;
        private readonly IReadOnlyRepository<DigitalContent> _readDigitalContentRepository;
        private readonly IReadOnlyRepository<MyDigitalContent> _readMyDigitalContentRepository;

        public SearchDigitalContentQueryHandler(
            IUserContext userContext,
            IReadUserReviewShared readUserReviewShared,
            IReadUserBookmarkShared readUserBookmarkShared,
            IReadOnlyRepository<DigitalContent> readDigitalContentRepository,
            IReadOnlyRepository<MyDigitalContent> readMyDigitalContentRepository) : base(userContext)
        {
            _readUserReviewShared = readUserReviewShared;
            _readUserBookmarkShared = readUserBookmarkShared;
            _readDigitalContentRepository = readDigitalContentRepository;
            _readMyDigitalContentRepository = readMyDigitalContentRepository;
        }

        protected override async Task<SearchPagedResultDto<DigitalContentModel, MyDigitalContentStatisticModel>> HandleAsync(
            SearchDigitalContentQuery query,
            CancellationToken cancellationToken)
        {
            var searchQuery = _readMyDigitalContentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            if (!string.IsNullOrEmpty(query.SearchText))
            {
                // Build query by SearchText condition.
                searchQuery = FilterBySearchTextQuery(query, searchQuery);
            }

            // Get total count of my digital content items by each my learning type
            var statistics =
                await CountByStatuses(query, searchQuery, cancellationToken);

            // Build query by digital content type
            searchQuery = FilterByDigitalContentType(query, searchQuery);

            // Build query by status
            searchQuery = FilterByStatusQuery(query, searchQuery);

            var totalCount = await searchQuery.CountAsync(cancellationToken);

            var sortCriteria = string.IsNullOrEmpty(query.OrderBy)
                ? $"{nameof(DigitalContent.CreatedDate)} DESC"
                : query.OrderBy;

            searchQuery = ApplySorting(searchQuery, query.PageInfo, sortCriteria);
            searchQuery = ApplyPaging(searchQuery, query.PageInfo);

            var myDigitalContents = await searchQuery.ToListAsync(cancellationToken);

            var digitalContentIds = myDigitalContents
                .Select(p => p.DigitalContentId)
                .Distinct()
                .ToList();

            var bookmarks = await _readUserBookmarkShared
                .GetByItemIds(CurrentUserIdOrDefault, digitalContentIds);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(digitalContentIds);

            var items = myDigitalContents.Select(mdc =>
            {
                var digitalContentId = mdc.DigitalContentId;
                var bookmark = bookmarks
                    .FirstOrDefault(p => p.ItemId == digitalContentId);

                var myDigitalContent = myDigitalContents
                    .FirstOrDefault(p => p.DigitalContentId == digitalContentId);

                return DigitalContentModel.New(
                        digitalContentId,
                        userReviewSummary[digitalContentId].AverageRating,
                        userReviewSummary[digitalContentId].ReviewCount)
                    .WithBookmarkInfo(bookmark)
                    .WithMyDigitalContent(myDigitalContent);
            }).ToList();

            return new SearchPagedResultDto<DigitalContentModel, MyDigitalContentStatisticModel>(
                totalCount,
                items,
                statistics);
        }

        /// <summary>
        /// Build query by digital content title.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">MyDigitalContent query.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="MyDigitalContent"/> information.</returns>
        private IQueryable<MyDigitalContent> FilterBySearchTextQuery(
            SearchDigitalContentQuery query,
            IQueryable<MyDigitalContent> searchQuery)
        {
            var digitalContentQuery = _readDigitalContentRepository
                .GetAll()
                .Where(p => p.Title.Contains(query.SearchText))
                .GroupBy(p => p.OriginalObjectId)
                .Select(p => new
                {
                    OriginalObjectId = p.Key
                });

            return searchQuery
                .Join(
                    digitalContentQuery,
                    mdc => mdc.DigitalContentId,
                    dc => dc.OriginalObjectId,
                    (myDigitalContent, digitalContent) => myDigitalContent);
        }

        /// <summary>
        /// Build query with <see cref="MyDigitalContentStatus"/>.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">MyDigitalContent query.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="MyDigitalContent"/> information.</returns>
        private IQueryable<MyDigitalContent> FilterByStatusQuery(
            SearchDigitalContentQuery query,
            IQueryable<MyDigitalContent> searchQuery)
        {
            if (query.StatusFilter == null)
            {
                return searchQuery;
            }

            switch (query.StatusFilter)
            {
                case MyDigitalContentStatus.InProgress:
                    return searchQuery
                        .Where(MyDigitalContent.FilterInProgressExpr());

                case MyDigitalContentStatus.Completed:
                    return searchQuery
                        .Where(MyDigitalContent.FilterCompletedExpr());

                default:
                    throw new UnexpectedStatusException($"{query.StatusFilter}");
            }
        }

        /// <summary>
        /// Build query with <see cref="DigitalContentType"/>.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">MyDigitalContent query.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="MyDigitalContent"/> information.</returns>
        private IQueryable<MyDigitalContent> FilterByDigitalContentType(
            SearchDigitalContentQuery query,
            IQueryable<MyDigitalContent> searchQuery)
        {
            if (query.DigitalContentType?.Count > 0)
            {
                return searchQuery
                    .Where(p => query.DigitalContentType.Contains(p.DigitalContentType));
            }

            return searchQuery;
        }

        /// <summary>
        /// Get total count of my digital content items by each my learning type.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">My digital content query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see cref="MyCourseStatisticModel"/> list
        /// to get total count of my digital content items
        /// by each <see cref="MyLearningStatus"/>.</returns>
        private async Task<List<MyDigitalContentStatisticModel>> CountByStatuses(
            SearchDigitalContentQuery query,
            IQueryable<MyDigitalContent> searchQuery,
            CancellationToken cancellationToken)
        {
            var statistics = new List<MyDigitalContentStatisticModel>();

            // If not include Statistic data -> we must be returned empty list data
            if (!query.IncludeStatistic || string.IsNullOrEmpty(query.SearchText))
            {
                return statistics;
            }

            var statusStatistic = await searchQuery
                .GroupBy(p => new
                {
                    p.Status
                })
                .Select(p => new
                {
                    p.Key.Status,
                    Total = p.Count()
                })
                .ToListAsync(cancellationToken);

            foreach (var status in query.StatusStatistics)
            {
                int statusCount = 0;

                switch (status)
                {
                    case MyDigitalContentStatus.InProgress:
                        statusCount = statusStatistic
                            .Where(p => p.Status == MyDigitalContentStatus.InProgress)
                            .Sum(p => p.Total);
                        break;

                    case MyDigitalContentStatus.Completed:
                        statusCount = statusStatistic
                            .Where(p => p.Status == MyDigitalContentStatus.Completed)
                            .Sum(p => p.Total);
                        break;

                    default:
                        throw new UnexpectedStatusException($"{query.StatusFilter}");
                }

                statistics.Add(new MyDigitalContentStatisticModel(status, statusCount));
            }

            return statistics;
        }
    }
}
