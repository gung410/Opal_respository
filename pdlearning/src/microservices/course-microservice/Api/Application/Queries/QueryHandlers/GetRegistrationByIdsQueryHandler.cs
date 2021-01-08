using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetRegistrationByIdsQueryHandler : BaseQueryHandler<GetRegistrationByIdsQuery, List<RegistrationModel>>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetRegistrationByIdsQueryHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override async Task<List<RegistrationModel>> HandleAsync(
            GetRegistrationByIdsQuery query,
            CancellationToken cancellationToken)
        {
            if (query.Ids == null || !query.Ids.Any())
            {
                return new List<RegistrationModel>();
            }

            var registrations = await _readRegistrationRepository
                .GetAll()
                .Where(x => query.Ids.Contains(x.Id))
                .Select(x => new RegistrationModel(x)).ToListAsync(cancellationToken);

            return registrations;
        }
    }
}
