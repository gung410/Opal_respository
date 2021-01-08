using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Uploader.Application.Queries.QueryHandlers
{
    public class GetPersonalSpaceByUserIdQueryHandler : BaseQueryHandler<GetPersonalSpaceByUserIdQuery, PersonalSpaceModel>
    {
        private readonly IRepository<PersonalSpace> _personalSpaceRepository;

        public GetPersonalSpaceByUserIdQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<PersonalSpace> personalFileRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(accessControlContext, unitOfWorkManager)
        {
            _personalSpaceRepository = personalFileRepository;
        }

        protected override async Task<PersonalSpaceModel> HandleAsync(GetPersonalSpaceByUserIdQuery query, CancellationToken cancellationToken)
        {
            var personalSpaceQuery = _personalSpaceRepository.GetAll();

            var personalSpace = await personalSpaceQuery.FirstOrDefaultAsync(ps => ps.UserId == query.UserId, cancellationToken);

            if (personalSpace == null)
            {
                throw new PersonalSpaceAccessDeniedException();
            }

            var fileModel = new PersonalSpaceModel(personalSpace);

            return fileModel;
        }
    }
}
