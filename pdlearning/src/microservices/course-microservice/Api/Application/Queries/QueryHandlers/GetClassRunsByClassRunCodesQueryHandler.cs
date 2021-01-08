using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetClassRunsByClassRunCodesQueryHandler : BaseQueryHandler<GetClassRunsByClassRunCodesQuery, List<ClassRunModel>>
    {
        private readonly GetClassRunsByClassRunCodesSharedQuery _getClassRunsByClassRunCodesSharedQuery;

        public GetClassRunsByClassRunCodesQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetClassRunsByClassRunCodesSharedQuery getClassRunsByClassRunCodesSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getClassRunsByClassRunCodesSharedQuery = getClassRunsByClassRunCodesSharedQuery;
        }

        protected override Task<List<ClassRunModel>> HandleAsync(
            GetClassRunsByClassRunCodesQuery query,
            CancellationToken cancellationToken)
        {
            return _getClassRunsByClassRunCodesSharedQuery.Execute(query.ClassRunCodes, query.CourseId, null, cancellationToken);
        }
    }
}
