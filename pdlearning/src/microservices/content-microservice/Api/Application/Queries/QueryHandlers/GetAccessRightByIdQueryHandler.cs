using System.Threading;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetAccessRightByIdQueryHandler : BaseThunderQueryHandler<GetAccessRightByIdQuery, AccessRightModel>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;

        public GetAccessRightByIdQueryHandler(IRepository<AccessRight> accessRightRepository)
        {
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<AccessRightModel> HandleAsync(GetAccessRightByIdQuery query, CancellationToken cancellationToken)
        {
            var accessRightEntity = await this._accessRightRepository.GetAsync(query.Id);
            AccessRightModel accessRightModel = new AccessRightModel(accessRightEntity);
            return accessRightModel;
        }
    }
}
