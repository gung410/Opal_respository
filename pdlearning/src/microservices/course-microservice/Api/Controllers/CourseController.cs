using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Controllers
{
    [Route("api/courses")]
    public class CourseController : BaseController<CourseService>
    {
        public CourseController(IUserContext userContext, CourseService appService) : base(userContext, appService)
        {
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<CourseModel>> SearchCourses([FromBody] SearchCoursesRequest request)
        {
            return await AppService.SearchCourses(request);
        }

        [HttpPost("searchUsers")]
        public async Task<PagedResultDto<UserModel>> SearchCourseUsers([FromBody] SearchCourseUsersRequest request)
        {
            return await AppService.SearchCourseUsers(request);
        }

        [HttpPost("getByIds")]
        public async Task<List<CourseModel>> GetListCoursesByListIds([FromBody] List<Guid> ids)
        {
            return await AppService.GetListCoursesByListIds(ids);
        }

        [HttpGet("{courseId:guid}")]
        public async Task<CourseModel> GetCourseDetailById(Guid courseId)
        {
            return await AppService.GetCourseDetailById(courseId);
        }

        [HttpPost("getByCourseCodes")]
        public async Task<IEnumerable<CourseModel>> GetCoursesByCourseCodes([FromBody] GetCoursesByCourseCodesRequest request)
        {
            return await AppService.GetCoursesByCourseCodes(request);
        }

        [HttpPut("changeStatus")]
        public async Task ChangeCourseStatus([FromBody] ChangeCourseStatusRequest request)
        {
            await AppService.ChangeCourseStatus(request);
        }

        [HttpPost("save")]
        public async Task<CourseModel> SaveCourse([FromBody] SaveCourseRequest request)
        {
            return await AppService.SaveCourse(request);
        }

        [HttpDelete("{courseId:guid}")]
        public async Task DeleteCourse(Guid courseId)
        {
            // Be aware of deleting course will cause removing all its children including sections and lectures.
            await AppService.DeleteCourse(courseId);
        }

        [HttpPost("clone")]
        public async Task<CourseModel> CloneCourse([FromBody] CloneCourseRequest request)
        {
            return await AppService.CloneCourse(request);
        }

        [HttpPost("migrateCourseNotification")]
        public async Task<PagedResultDto<Guid>> MigrateCourseNotification([FromBody] MigrateCourseNotificationRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            return await AppService.MigrateCourseNotification(request);
        }

        [HttpPost("checkExistedCourseField")]
        public async Task<bool> CheckExistedCourseField([FromBody] CheckExistedCourseFieldRequest request)
        {
            return await AppService.CheckExistedCourseField(request);
        }

        [HttpPost("checkCourseEndDateValidWithClassEndDate")]
        public async Task<bool> CheckCourseEndDateValidWithClassEndDate([FromBody] CheckCourseEndDateValidWithClassEndDateRequest request)
        {
            return await AppService.CheckCourseEndDateValidWithClassEndDate(request);
        }

        [HttpPut("transfer")]
        public async Task TransferCourseOwnership([FromBody] TransferCourseOwnershipRequest request)
        {
            await AppService.TransferCourseOwnership(request);
        }

        [HttpPut("archive")]
        public async Task ArchiveCourse([FromBody] ArchiveCourseRequest request)
        {
            await AppService.ArchiveCourse(request);
        }
    }
}
