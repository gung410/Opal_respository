using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
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
            var dbQuery = _accessRightRepository
                .GetAll()
                .Where(c => c.ObjectId == query.Request.OriginalObjectId);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = await dbQuery
                .Select(p => new AccessRightModel(p))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<AccessRightModel>(totalCount, entities);
        }
    }
}
