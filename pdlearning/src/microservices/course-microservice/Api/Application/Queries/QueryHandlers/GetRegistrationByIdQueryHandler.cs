using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetRegistrationByIdQueryHandler : BaseQueryHandler<GetRegistrationByIdQuery, RegistrationModel>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetRegistrationByIdQueryHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override async Task<RegistrationModel> HandleAsync(
            GetRegistrationByIdQuery query,
            CancellationToken cancellationToken)
        {
            var registration = await _readRegistrationRepository.GetAsync(query.Id);

            return new RegistrationModel(registration);
        }
    }
}
