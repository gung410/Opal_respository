using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class SearchAccessRightQueryHandler : BaseThunderQueryHandler<SearchAccessRightQuery, PagedResultDto<AccessRightModel>>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;

        public SearchAccessRightQueryHandler(IRepository<AccessRight> accessRightRepository)
        {
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<PagedResultDto<AccessRightModel>> HandleAsync(SearchAccessRightQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _accessRightRepository.GetAll()
                .Where(c => c.ObjectId == query.OriginalObjectId);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);
            dbQuery = ApplyPaging(dbQuery, query.PagedInfo);

            var entities = await dbQuery.Select(p => new AccessRightModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<AccessRightModel>(totalCount, entities);
        }
    }
}
