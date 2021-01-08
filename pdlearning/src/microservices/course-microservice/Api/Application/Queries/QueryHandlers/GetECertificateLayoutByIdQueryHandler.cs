using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetECertificateLayoutByIdQueryHandler : BaseQueryHandler<GetECertificateLayoutByIdQuery, ECertificateLayoutModel>
    {
        public GetECertificateLayoutByIdQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
        }

        protected override Task<ECertificateLayoutModel> HandleAsync(GetECertificateLayoutByIdQuery query, CancellationToken cancellationToken)
        {
            var ecertificateLayout = EnsureEntityFound(ECertificateLayoutConstant
                .AllECertificateLayoutsInSystem
                .FirstOrDefault(p => p.Id == query.Id));

            return Task.FromResult(
                new ECertificateLayoutModel(
                    ecertificateLayout,
                    !string.IsNullOrWhiteSpace(ecertificateLayout.PreviewImagePath) ? Convert.ToBase64String(File.ReadAllBytes(ecertificateLayout.PreviewImagePath)) : string.Empty));
        }
    }
}
