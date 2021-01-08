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
    public class GetBlockoutDateDependenciesQueryHandler : BaseQueryHandler<GetBlockoutDateDependenciesQuery, GetBlockoutDateDependenciesModel>
    {
        private readonly GetBlockoutDateDependenciesSharedQuery _getBlockoutDateDependenciesSharedQuery;

        public GetBlockoutDateDependenciesQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetBlockoutDateDependenciesSharedQuery getBlockoutDateDependenciesSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getBlockoutDateDependenciesSharedQuery = getBlockoutDateDependenciesSharedQuery;
        }

        protected override Task<GetBlockoutDateDependenciesModel> HandleAsync(GetBlockoutDateDependenciesQuery query, CancellationToken cancellationToken)
        {
            return _getBlockoutDateDependenciesSharedQuery.Execute(query.ServiceSchemes, query.FromDate, query.ToDate, cancellationToken);
        }
    }
}
