using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.Extensions.Options;
using Telerik.Reporting;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetPreviewECertificateTemplateQueryHandler : BaseQueryHandler<GetPreviewECertificateTemplateQuery, PreviewECertificateTemplateModel>
    {
        private readonly ECertificateBuilderLogic _certificateBuilder;

        public GetPreviewECertificateTemplateQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            ECertificateBuilderLogic certificateBuilder) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _certificateBuilder = certificateBuilder;
        }

        protected override async Task<PreviewECertificateTemplateModel> HandleAsync(GetPreviewECertificateTemplateQuery query, CancellationToken cancellationToken)
        {
            var ecertificateLayout = EnsureEntityFound(ECertificateLayoutConstant.AllECertificateLayoutsInSystem.FirstOrDefault(p => p.Id == query.ECertificateLayoutId));

            return new PreviewECertificateTemplateModel
            {
                Base64ECertificateTemplate =
                    await _certificateBuilder.BuildForLayoutBase64(
                        ecertificateLayout,
                        ReportGeneralOutputFormatType.IMAGE,
                        query.Params.ToList(),
                        null,
                        cancellationToken),
                Width = ecertificateLayout.Width,
                Height = ecertificateLayout.Height
            };
        }
    }
}
