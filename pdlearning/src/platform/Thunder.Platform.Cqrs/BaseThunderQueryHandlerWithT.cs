using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Extensions;

namespace Thunder.Platform.Cqrs
{
#pragma warning disable SA1649 // File name should match first type name
    public abstract class BaseThunderQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
#pragma warning restore SA1649 // File name should match first type name
        where TQuery : BaseThunderQuery<TResult>
    {
        public virtual async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            return await HandleAsync(request, cancellationToken);
        }

        protected abstract Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);

        protected virtual IQueryable<TEntity> ApplySorting<TEntity, TInput>(
            IQueryable<TEntity> query,
            TInput input,
            string defaultSortCriteria = "Id DESCENDING") where TEntity : class
        {
            // Try to sort query if available
            if (input is ISortedResultRequest sortInput)
            {
                if (!string.IsNullOrWhiteSpace(sortInput.Sorting))
                {
                    return query.OrderBy(sortInput.Sorting);
                }
            }

            // IQueryable.Take requires sorting, so we should sort if Take will be used.
            if (input is ILimitedResultRequest)
            {
                // This will throw exception if there is no Id column in entity class
                // This will happened with PiManagement table because this table doesn't have Id column
                return query.OrderBy(defaultSortCriteria);
            }

            // No sorting
            return query;
        }

        protected virtual IQueryable<TEntity> ApplyPaging<TEntity, TInput>(IQueryable<TEntity> query, TInput input)
            where TEntity : class
        {
            // Try to use paging if available
            if (input is IPagedResultRequest pagedInput)
            {
                return query.PageBy(pagedInput);
            }

            // Try to limit query result if available
            if (input is ILimitedResultRequest limitedInput)
            {
                return query.Take(limitedInput.MaxResultCount);
            }

            // No paging
            return query;
        }
    }
}
