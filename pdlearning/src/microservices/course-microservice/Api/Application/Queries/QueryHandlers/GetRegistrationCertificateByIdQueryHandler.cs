using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetRegistrationCertificateByIdQueryHandler : BaseQueryHandler<GetRegistrationCertificateByIdQuery, RegistrationECertificateModel>
    {
        private readonly IReadOnlyRepository<RegistrationECertificate> _readRegistrationECertificateRepository;

        public GetRegistrationCertificateByIdQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<RegistrationECertificate> readRegistrationECertificateRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationECertificateRepository = readRegistrationECertificateRepository;
        }

        protected override async Task<RegistrationECertificateModel> HandleAsync(GetRegistrationCertificateByIdQuery query, CancellationToken cancellationToken)
        {
            var registrationECertificate = await _readRegistrationECertificateRepository.GetAsync(query.RegistrationId);

            return RegistrationECertificateModel.Create(registrationECertificate);
        }
    }
}
