using System;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.RequestDtos.AnnouncementRequest;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/announcement")]
    public class AnnouncementController : BaseController<AnnouncementService>
    {
        public AnnouncementController(IUserContext userContext, AnnouncementService appService) : base(userContext, appService)
        {
        }

        [HttpGet("searchTemplate")]
        public async Task<PagedResultDto<AnnouncementTemplateModel>> SearchAnnouncementTemplates(SearchAnnouncementTemplateRequest request)
        {
            return await AppService.SearchAnnouncementTemplates(request);
        }

        [HttpPost("saveTemplate")]
        public async Task<AnnouncementTemplateModel> SaveAnnouncementTemplate([FromBody] SaveAnnouncementTemplateRequest request)
        {
            return await AppService.SaveAnnouncementTemplate(request);
        }

        [HttpDelete("{announcementTemplateId:guid}")]
        public async Task DeleteAnnouncementTemplate(Guid announcementTemplateId)
        {
            await AppService.DeleteAnnouncementTemplate(announcementTemplateId);
        }

        [HttpPost("sendAnnouncement")]
        public async Task SendAnnouncement([FromBody] SendAnnouncementRequest request)
        {
            await AppService.SendAnnouncement(request, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<AnnouncementModel>> SearchAnnouncement([FromBody] SearchAnnouncementRequest request)
        {
            return await AppService.SearchAnnouncement(request);
        }

        [HttpPut("changeStatus")]
        public async Task ChangeAnnouncementStatus([FromBody] ChangeAnnouncementStatusRequest request)
        {
            await AppService.ChangeAnnouncementStatus(request);
        }

        [HttpPost("getSendAnnouncementDefaultTemplate")]
        public async Task<SendAnnouncementEmailTemplateModel> GetSendAnnouncementDefaultTemplate([FromBody] GetSendAnnouncementDefaultTemplateRequest request)
        {
            return await AppService.GetSendAnnouncementDefaultTemplate(request);
        }

        [HttpPost("previewAnnouncementTemplate")]
        public async Task<PreviewAnnouncementTemplateModel> PreviewAnnouncementTemplate([FromBody] PreviewAnnouncementTemplateRequest request)
        {
            return await AppService.PreviewAnnouncementTemplate(request);
        }

        [HttpPost("sendCoursePublicity")]
        public async Task SendCoursePublicity([FromBody] SendCoursePublicityRequest request)
        {
            await AppService.SendCoursePublicity(request);
        }

        [HttpPost("sendCourseNominationAnnoucement")]
        public async Task SendCourseNomination([FromBody] SendCourseNominationAnnoucementRequest request)
        {
            await AppService.SendCourseAnnoucementNomination(request);
        }

        [HttpPost("sendPlacementLetter")]
        public async Task SendPlacementLetter([FromBody] SendPlacementLetterRequest request)
        {
            await AppService.SendPlacementLetter(request);
        }

        [HttpPost("sendOrderRefreshment")]
        public async Task SendOrderRefreshment([FromBody] SendOrderRefreshmentRequest request)
        {
            await AppService.SendOrderRefreshment(request);
        }
    }
}
