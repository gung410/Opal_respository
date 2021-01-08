using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/ecertificate")]
    public class ECertificateController : BaseController<ECertificateService>
    {
        public ECertificateController(IUserContext userContext, ECertificateService appService) : base(userContext, appService)
        {
        }

        [HttpGet("template/search")]
        public async Task<PagedResultDto<ECertificateTemplateModel>> SearchECertificateTemplates(SearchECertificatesRequest request)
        {
            return await AppService.SearchECertificateTemplates(request);
        }

        [HttpGet("template/{ecertificateId:guid}")]
        public async Task<ECertificateTemplateModel> GetEcertificateTemplateById(Guid ecertificateId)
        {
            return await AppService.GetECertificateTemplateById(ecertificateId);
        }

        [HttpPost("template/preview")]
        public async Task<PreviewECertificateTemplateModel> GetPreviewECertificateTemplate([FromBody] GetPreviewECertificateTemplateRequest templateRequest)
        {
            return await AppService.GetPreviewECertificateTemplate(templateRequest);
        }

        [HttpPost("template/save")]
        public async Task<ECertificateTemplateModel> SaveECertificateTemplate([FromBody] SaveECertificateTemplateRequest request)
        {
            return await AppService.SaveECertificateTemplate(request);
        }

        [HttpGet("layout/all")]
        public async Task<IEnumerable<ECertificateLayoutModel>> GetECertificateLayouts()
        {
            return await AppService.GetECertificateLayouts();
        }

        [HttpGet("layout/{id:guid}")]
        public async Task<ECertificateLayoutModel> GetECertificateLayoutById(Guid id)
        {
            return await AppService.GetECertificateLayoutById(id);
        }

        [HttpDelete("{templateId:guid}")]
        public async Task DeleteEcertificateTemplate(Guid templateId)
        {
            await AppService.DeleteECertificateTemplate(templateId);
        }

        [HttpGet("{registrationId:guid}/download-ecertificate")]
        public async Task<IActionResult> DownloadRegistrationECertificate(Guid registrationId, [FromQuery] ReportGeneralOutputFormatType fileFormat)
        {
            var eCertificateRegistrationResult = await AppService.DownloadRegistrationECertificate(registrationId, fileFormat);

            return DownloadFile(
                eCertificateRegistrationResult.FileContent,
                fileFormat == ReportGeneralOutputFormatType.IMAGE ? "image/jpeg" : "application/pdf",
                eCertificateRegistrationResult.FileName);
        }
    }
}
