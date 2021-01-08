using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class DownloadRegistrationECertificateQueryHandler : BaseQueryHandler<DownloadRegistrationECertificateQuery, DownloadECertificateRegistrationResultModel>
    {
        private readonly IReadOnlyRepository<RegistrationECertificate> _readRegistrationECertificateRepository;

        public DownloadRegistrationECertificateQueryHandler(
            IReadOnlyRepository<RegistrationECertificate> readRegistrationECertificateRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationECertificateRepository = readRegistrationECertificateRepository;
        }

        protected override async Task<DownloadECertificateRegistrationResultModel> HandleAsync(DownloadRegistrationECertificateQuery query, CancellationToken cancellationToken)
        {
            var registrationECertificate = await _readRegistrationECertificateRepository.GetAsync(query.RegistrationId);

            switch (query.FileFormat)
            {
                case ReportGeneralOutputFormatType.IMAGE:
                    return new DownloadECertificateRegistrationResultModel
                    {
                        FileContent = Convert.FromBase64String(registrationECertificate.Base64Image),
                        FileFormat = query.FileFormat,
                        FileName = registrationECertificate.ImageFileName
                    };

                default:
                    return new DownloadECertificateRegistrationResultModel
                    {
                        FileContent = Convert.FromBase64String(registrationECertificate.Base64Pdf),
                        FileFormat = query.FileFormat,
                        FileName = registrationECertificate.PdfFileName
                    };
            }
        }
    }
}
