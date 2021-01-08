using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Controllers
{
    [Route("api/learningcontent")]
    public class LearningContentController : BaseController<LearningContentService>
    {
        public LearningContentController(IUserContext userContext, LearningContentService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{courseId:guid}/lectures/getIds")]
        public async Task<List<Guid>> GetAllLectureIdsByCourseId(Guid courseId)
        {
            return await AppService.GetAllLectureIdsByCourseId(courseId);
        }

        [HttpGet("{courseId:guid}/toc")]
        public async Task<List<ContentItem>> GetTableOfContent(Guid courseId, [FromQuery] Guid? classRunId, [FromQuery] string searchText, [FromQuery] bool includeAdditionalInfo)
        {
            // TOC: table of content.
            // Refer: https://www.abbreviations.com/abbreviation/table%20of%20content
            return await AppService.GetTableOfContent(courseId, classRunId, searchText, includeAdditionalInfo);
        }

        [HttpPut("{courseId:guid}/toc/changeContentOrder")]
        public async Task<List<ContentItem>> ChangeContentOrder([FromBody] ChangeContentOrderRequest request)
        {
            return await AppService.ChangeContentOrder(request);
        }

        [HttpPut("changeCourseContentStatus")]
        public async Task ChangeCourseContentStatus([FromBody]ChangeCourseContentStatusRequest request)
        {
            await AppService.ChangeCourseContentStatus(request);
        }

        [HttpPut("changeClassRunContentStatus")]
        public async Task ChangeClassRunContentStatus([FromBody] ChangeClassRunContentStatusRequest request)
        {
            await AppService.ChangeClassRunContentStatus(request);
        }

        [HttpPost("cloneContentForClassRun")]
        public async Task<List<ContentItem>> CloneContentForClassRun([FromBody] CloneContentForClassRunRequest request)
        {
            return await AppService.CloneContentForClassRun(request);
        }

        [HttpPost("cloneContentForCourse")]
        public async Task<List<ContentItem>> CloneContentForCourse([FromBody] CloneContentForCourseRequest request)
        {
            return await AppService.CloneContentForCourse(request);
        }

        [HttpGet("sections/{sectionId:guid}")]
        public async Task<SectionModel> GetSectionById(Guid sectionId)
        {
            return await AppService.GetSectionById(sectionId);
        }

        [HttpPost("sections/save")]
        public async Task<SectionModel> CreateOrUpdateSection([FromBody] CreateOrUpdateSectionRequest request)
        {
            return await AppService.CreateOrUpdateSection(request);
        }

        [HttpDelete("{courseId:guid}/sections/{sectionId:guid}")]
        public async Task DeleteSection(Guid courseId, Guid sectionId)
        {
            // Be aware of deleting an section will cause removing all its children including lectures.
            await AppService.DeleteSection(courseId, sectionId);
        }

        [HttpGet("lectures/{lectureId:guid}")]
        public async Task<LectureModel> GetLectureById(Guid lectureId)
        {
            return await AppService.GetLectureById(lectureId);
        }

        [HttpPost("lectures/save")]
        public async Task<LectureModel> SaveLecture([FromBody] SaveLectureRequest request)
        {
            return await AppService.SaveLecture(request);
        }

        [HttpDelete("{courseId:guid}/lectures/{lectureId:guid}")]
        public async Task DeleteLecture(Guid courseId, Guid lectureId)
        {
            await AppService.DeleteLecture(courseId, lectureId);
        }

        [HttpPost("lectures/getAllNamesByListIds")]
        public async Task<LectureIdMapNameModel[]> GetAllLectureNamesByListIds([FromBody] Guid[] listLectureIds)
        {
            return await AppService.GetAllLectureNamesByListIds(listLectureIds);
        }

        [HttpPost("digitalContents/getAllIdsBelongToCourseIds")]
        public async Task<CourseIdMapListDigitalContentIdModel[]> GetAllDigitalContentIdsInCourse([FromBody] Guid[] listCourseIds)
        {
            return await AppService.GetAllDigitalContentIdsBelongTheseCourseIds(listCourseIds);
        }

        [HttpPost("hasReferenceToResource")]
        public async Task<bool> HasReferenceToResource([FromBody] HasReferenceToResourceRequest request)
        {
            return await AppService.HasReferenceToResource(request);
        }

        [HttpPost("migrateContentNotification")]
        public async Task MigrateCourseNotification([FromBody] MigrateContentNotificationRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            await AppService.MigrateContentNotification(request);
        }
    }
}
