using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetClassRunRemainingSlotQueryHandler : BaseQueryHandler<GetClassRunRemainingSlotQuery, Dictionary<Guid, int>>
    {
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;

        public GetClassRunRemainingSlotQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
        }

        protected override Task<Dictionary<Guid, int>> HandleAsync(GetClassRunRemainingSlotQuery query, CancellationToken cancellationToken)
        {
            return _getRemainingClassRunSlotSharedQuery.ByClassRunIds(query.ClassRunIds.ToList(), cancellationToken);
        }
    }
}
