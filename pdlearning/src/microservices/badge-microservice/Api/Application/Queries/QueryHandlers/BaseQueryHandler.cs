using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public abstract class BaseQueryHandler<TQuery, TResult> : BaseThunderQueryHandler<TQuery, TResult> where TQuery : BaseThunderQuery<TResult>
    {
        private readonly IUserContext _userContext;

        protected BaseQueryHandler(IUserContext userContext, BadgeDbContext dbContext)
        {
            _userContext = userContext;
            DbContext = dbContext;
        }

        protected Guid? CurrentUserId
        {
            get => _userContext.CurrentUserId();
        }

        protected BadgeDbContext DbContext { get; }

        protected async Task<PagedResultDto<TPagedResult>> ApplyMongoPaging<TQueryHandler, TPagedResult>(
            IAggregateFluent<TQueryHandler> query,
            PagedResultRequestDto pageInfo,
            [NotNull] Expression<Func<TQueryHandler, TPagedResult>> mapperModelExpr,
            CancellationToken cancellationToken = default)
        {
            var totalItems = await query.CountAsync(cancellationToken);

            var items = (pageInfo?.MaxResultCount ?? 0) > 0
                ? await query.Skip(pageInfo.SkipCount).Limit(pageInfo.MaxResultCount).Select(mapperModelExpr).ToListAsync(cancellationToken)
                : new List<TPagedResult>();

            return new PagedResultDto<TPagedResult>
            {
                Items = items,
                TotalCount = totalItems
            };
        }

        protected Task<PagedResultDto<TPagedResult>> ApplyMongoPaging<TQueryHandler, TPagedResult>(
            IMongoQueryable<TQueryHandler> query,
            PagedResultRequestDto pageInfo,
            Expression<Func<TQueryHandler, TPagedResult>> mapperModel,
            CancellationToken cancellationToken = default)
        {
            var itemsQuery = query.Select(mapperModel);
            return ApplyMongoPaging(itemsQuery, pageInfo, cancellationToken);
        }

        protected async Task<PagedResultDto<TQueryHandler>> ApplyMongoPaging<TQueryHandler>(
            IMongoQueryable<TQueryHandler> query,
            PagedResultRequestDto pageInfo,
            CancellationToken cancellationToken)
        {
            var totalItems = await query.CountAsync(cancellationToken);
            var pagedItems = (pageInfo?.MaxResultCount ?? 0) > 0
                ? await query.Skip(pageInfo.SkipCount).Take(pageInfo.MaxResultCount).ToListAsync(cancellationToken)
                : new List<TQueryHandler>();

            return new PagedResultDto<TQueryHandler>
            {
                Items = pagedItems,
                TotalCount = totalItems
            };
        }

        protected async Task<PagedResultDto<TPagedResult>> ApplyMongoPagingSelectClient<TQueryHandler, TPagedResult>(
            IMongoQueryable<TQueryHandler> query,
            PagedResultRequestDto pageInfo,
            [NotNull] Func<TQueryHandler, TPagedResult> mapperModelExpr,
            CancellationToken cancellationToken = default)
        {
            var totalItems = await query.CountAsync(cancellationToken);

            var items = (pageInfo?.MaxResultCount ?? 0) > 0
                ? (await query.Skip(pageInfo.SkipCount).Take(pageInfo.MaxResultCount).ToListAsync(cancellationToken)).SelectList(mapperModelExpr)
                : new List<TPagedResult>();

            return new PagedResultDto<TPagedResult>
            {
                Items = items,
                TotalCount = totalItems
            };
        }

        protected IMongoQueryable<TQueryHandler> ApplyMongoPaging<TQueryHandler>(
            IMongoQueryable<TQueryHandler> query,
            PagedResultRequestDto pageInfo)
        {
            return query
                .Skip(pageInfo.SkipCount)
                .Take(pageInfo.MaxResultCount);
        }

        protected async Task<PagedResultDto<TPagedResult>> ApplyMongoPaging<TPagedResult>(
            IAggregateFluent<TPagedResult> aggregateFluent,
            PagedResultRequestDto pageInfo,
            CancellationToken cancellationToken = default)
        {
            var totalItems = await aggregateFluent.CountAsync(cancellationToken);

            var pagedQuery = await aggregateFluent
                .Skip(pageInfo.SkipCount)
                .Limit(pageInfo.MaxResultCount)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TPagedResult>
            {
                Items = pagedQuery,
                TotalCount = totalItems
            };
        }

        /// <summary>
        /// This method is used to get result from two queries after apply paging.
        /// </summary>
        /// <typeparam name="TQueryResult">Type of query.</typeparam>
        /// <param name="firstQuery">The query will be handled first.</param>
        /// <param name="secondQuery">The query will be handled after handling first Query.</param>
        /// <param name="pageInfo">Paging Info.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Result which be handled from  both two queries.</returns>
        protected async Task<PagedResultDto<TQueryResult>> ApplyMongoPagingQueries<TQueryResult>(
            IAggregateFluent<TQueryResult> firstQuery,
            IAggregateFluent<TQueryResult> secondQuery,
            PagedResultRequestDto pageInfo,
            CancellationToken cancellationToken = default)
        {
            List<TQueryResult> pagedSecondQueryResult = new();

            var firstQueryTotal = await firstQuery.CountAsync(cancellationToken);
            var secondQueryTotal = await secondQuery.CountAsync(cancellationToken);
            var totalCount = firstQueryTotal + secondQueryTotal;
            var resultLimit = pageInfo?.MaxResultCount ?? 0;
            if (resultLimit == 0)
            {
                return new PagedResultDto<TQueryResult>(totalCount);
            }

            var skip = pageInfo?.SkipCount ?? 0;
            var secondQuerySkip = skip - firstQueryTotal;

            // Need to get more result from first query.
            if (secondQuerySkip < 0)
            {
                var pagedFirstQueryResult = await firstQuery
                    .Skip(skip)
                    .Limit(resultLimit)
                    .ToListAsync(cancellationToken);

                var secondQueryLimit = resultLimit - pagedFirstQueryResult.Count;
                if (secondQueryLimit > 0)
                {
                    pagedSecondQueryResult = await secondQuery
                        .Limit(secondQueryLimit)
                        .ToListAsync(cancellationToken);
                }

                return new PagedResultDto<TQueryResult>(
                    totalCount,
                    pagedSecondQueryResult.Union(pagedFirstQueryResult).ToList());
            }

            pagedSecondQueryResult = await secondQuery
                .Skip(secondQuerySkip)
                .Limit(resultLimit)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<TQueryResult>(
                totalCount,
                pagedSecondQueryResult);
        }
    }
}
