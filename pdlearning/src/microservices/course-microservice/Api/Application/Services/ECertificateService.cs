using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class ECertificateService : BaseApplicationService
    {
        public ECertificateService(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWork) : base(thunderCqrs, unitOfWork)
        {
        }

        public Task<PagedResultDto<ECertificateTemplateModel>> SearchECertificateTemplates(SearchECertificatesRequest request)
        {
            return ThunderCqrs.SendQuery(
                new SearchECertificateTemplatesQuery
                {
                    SearchText = request.SearchText,
                    SearchType = request.SearchType,
                    PageInfo = new PagedResultRequestDto()
                    {
                        SkipCount = request.SkipCount,
                        MaxResultCount = request.MaxResultCount
                    }
                });
        }

        public Task<ECertificateTemplateModel> GetECertificateTemplateById(Guid eCertificateId)
        {
            return ThunderCqrs.SendQuery(new GetECertificateTemplateByIdQuery { Id = eCertificateId });
        }

        public Task<IEnumerable<ECertificateLayoutModel>> GetECertificateLayouts()
        {
            return ThunderCqrs.SendQuery(new GetECertificateLayoutsQuery());
        }

        public Task<ECertificateLayoutModel> GetECertificateLayoutById(Guid id)
        {
            return ThunderCqrs.SendQuery(new GetECertificateLayoutByIdQuery
            {
                Id = id
            });
        }

        public Task<PreviewECertificateTemplateModel> GetPreviewECertificateTemplate(GetPreviewECertificateTemplateRequest templateRequest)
        {
            return ThunderCqrs.SendQuery(new GetPreviewECertificateTemplateQuery
            {
                ECertificateLayoutId = templateRequest.ECertificateLayoutId,
                Params = templateRequest.Params
            });
        }

        public async Task<ECertificateTemplateModel> SaveECertificateTemplate(SaveECertificateTemplateRequest request)
        {
            var command = request.Data.ToCommand();
            await ThunderCqrs.SendCommand(command);
            return await ThunderCqrs.SendQuery(new GetECertificateTemplateByIdQuery { Id = command.Id });
        }

        public Task DeleteECertificateTemplate(Guid templateId)
        {
            return ThunderCqrs.SendCommand(new DeleteECertificateTemplateCommand { ECertificateTemplateId = templateId });
        }

        public Task<DownloadECertificateRegistrationResultModel> DownloadRegistrationECertificate(Guid registrationId, ReportGeneralOutputFormatType fileFormat)
        {
            return ThunderCqrs.SendQuery(
                new DownloadRegistrationECertificateQuery
                {
                    RegistrationId = registrationId,
                    FileFormat = fileFormat
                });
        }
    }
}
