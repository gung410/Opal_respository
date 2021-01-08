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
    public class GetECertificateTemplateByIdQueryHandler : BaseQueryHandler<GetECertificateTemplateByIdQuery, ECertificateTemplateModel>
    {
        private readonly IReadOnlyRepository<ECertificateTemplate> _readEcertificateTemplateRepository;

        public GetECertificateTemplateByIdQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ECertificateTemplate> readEcertificateTemplateRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readEcertificateTemplateRepository = readEcertificateTemplateRepository;
        }

        protected override async Task<ECertificateTemplateModel> HandleAsync(
            GetECertificateTemplateByIdQuery query,
            CancellationToken cancellationToken)
        {
            var ecertificateTemplate = await _readEcertificateTemplateRepository.GetAsync(query.Id);
            return new ECertificateTemplateModel(ecertificateTemplate);
        }
    }
}
