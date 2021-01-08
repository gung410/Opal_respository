using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Models;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetListDigitalContentsByListIdQueryHandler : BaseQueryHandler<GetListDigitalContentsByListIdQuery, List<DigitalContentModel>>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetListDigitalContentsByListIdQueryHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<DigitalContent> digitalContentRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override Task<List<DigitalContentModel>> HandleAsync(GetListDigitalContentsByListIdQuery query, CancellationToken cancellationToken)
        {
            return _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasPermissionToSeeContentExpr(CurrentUserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId)
                .Where(_ => query.ListIds.Contains(_.Id))
                .Select(p => new DigitalContentModel(p))
                .ToListAsync(cancellationToken);
        }
    }
}
