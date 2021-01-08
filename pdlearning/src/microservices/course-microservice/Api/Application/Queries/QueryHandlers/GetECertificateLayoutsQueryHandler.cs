using System;
using System.Collections.Generic;
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
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetECertificateLayoutsQueryHandler : BaseQueryHandler<GetECertificateLayoutsQuery, IEnumerable<ECertificateLayoutModel>>
    {
        public GetECertificateLayoutsQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
        }

        protected override Task<IEnumerable<ECertificateLayoutModel>> HandleAsync(GetECertificateLayoutsQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(ECertificateLayoutConstant.AllECertificateLayoutsInSystem
                .Where(x => x.IsSystem == false)
                .Select(x =>
                    new ECertificateLayoutModel(x, !string.IsNullOrWhiteSpace(x.PreviewImagePath) ? Convert.ToBase64String(File.ReadAllBytes(x.PreviewImagePath)) : string.Empty)));
        }
    }
}
