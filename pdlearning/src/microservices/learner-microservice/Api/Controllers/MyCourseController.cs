using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Controllers
{
    // Use 'me' keyword like OneDrive REST API
    // https://docs.microsoft.com/en-us/onedrive/developer/rest-api/api/drive_get?view=odsp-graph-online
    [Route("api/me/courses")]
    public class MyCourseController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyCourseController(IUserContext userContext, IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [PermissionRequired(LearnerPermissionKeys.ViewCourse, LearnerPermissionKeys.ViewMicrolearning)]
        [HttpPost("search")]
        public async Task<SearchPagedResultDto<CourseModel, MyCourseStatisticModel>> SearchMyCourse([FromBody] SearchMyCourseRequestDto request)
        {
            var query = new SearchMyCourseQuery
            {
                CourseType = request.CourseType,
                SearchText = request.SearchText,
                IncludeStatistic = request.IncludeStatistic,
                MyLearningStatusFilter = request.StatusFilter,
                MyLearningStatusStatistic = request.StatisticsFilter,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewCourse, LearnerPermissionKeys.ViewMicrolearning)]
        [HttpGet("")]
        public async Task<PagedResultDto<CourseModel>> GetMyCourses(GetMyCoursesRequestDto request)
        {
            var query = new GetMyCoursesQuery
            {
                StatusFilter = request.StatusFilter,
                OrderBy = request.OrderBy,
                CourseId = request.CourseId,
                CourseType = request.CourseType,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("summary")]
        public async Task<List<MyCoursesSummaryModel>> GetSummaryMyCourse([FromBody] GetMyCoursesSummaryRequestDto request)
        {
            var query = new GetMyCoursesSummaryQuery
            {
                StatusFilter = request.StatusFilter
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewUserBookmark)]
        [HttpGet("bookmarks")]
        public async Task<PagedResultDto<CourseModel>> GetUserBookmarks(GetUserBookmarkRequestDto request)
        {
            var query = new GetUserBookmarksQuery
            {
                BookmarkTypeFilter = request.BookmarkTypeFilter,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("getByCourseIds")]
        public async Task<List<CourseModel>> GetMyCoursesByCourseIds([FromBody] GetCoursesByIdsRequestDto dto)
        {
            var query = new GetCoursesByCourseIdsQuery
            {
                CourseIds = dto.CourseIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewCourse, LearnerPermissionKeys.ViewMicrolearning)]
        [HttpGet("{myCourseId:guid}/lectures")]
        public async Task<List<LectureInMyCourseModel>> GetLecturesInMyCourse(Guid myCourseId)
        {
            var query = new GetLecturesInMyCourseQuery
            {
                MyCourseId = myCourseId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewCourse, LearnerPermissionKeys.ViewMicrolearning)]
        [HttpGet("details/byCourseId/{courseId:guid}")]
        public async Task<CourseModel> GetMyCourseDetailsByCourseId(Guid courseId)
        {
            var query = new GetMyCourseDetailsByCourseIdQuery
            {
                CourseId = courseId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPost("enroll")]
        public async Task<MyCourseModel> EnrollCourse([FromBody] EnrollCourseRequestDto dto)
        {
            var command = new EnrollCourseCommand
            {
                CourseId = dto.CourseId,
                LectureIds = dto.LectureIds
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetMyCourseByCourseIdQuery
            {
                CourseId = dto.CourseId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPost("reEnroll")]
        public async Task<MyCourseModel> ReEnrollCourse([FromBody] ReEnrollCourseRequestDto dto)
        {
            var command = new ReEnrollCourseCommand
            {
                Id = Guid.NewGuid(),
                CourseId = dto.CourseId,
                LectureIds = dto.LectureIds,
                CourseType = dto.CourseType
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetMyCourseByCourseIdQuery
            {
                CourseId = dto.CourseId
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPut("lectures/complete/{lectureInMyCourseId:guid}")]
        public async Task CompleteMyCourseLecture(Guid lectureInMyCourseId)
        {
            var command = new CompleteLectureInMyCourseCommand
            {
                LectureInMyCourseId = lectureInMyCourseId
            };

            await _thunderCqrs.SendCommand(command);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPut("updateStatus")]
        public async Task<MyCourseModel> UpdateCourseStatus([FromBody] UpdateCourseStatusRequestDto dto)
        {
            var command = new UpdateMyCourseStatusCommand
            {
                CourseId = dto.CourseId,
                Status = dto.Status
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetMyCourseByCourseIdQuery
            {
                CourseId = dto.CourseId
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("migratePdRecord")]
        public Task<int> GetMyCourseToTriggerPdRecordEvent([FromBody] MigratePdRecordRequestDto dto)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new UnauthorizedAccessException("The user must be in role SysAdministrator!");
            }

            return _thunderCqrs.SendQuery(new GetMyCourseToMigrateMicroLearningQuery
            {
                CourseIds = dto.CourseIds,
                BatchSize = dto.BatchSize,
                Statuses = dto.Statuses,
                MigrationEventType = dto.MigrationEventType
            });
        }

        [HttpPost("migrateLearningProcess")]
        public async Task TriggerLearningProcessEvent([FromBody] MigrateLearningProcessRequestDto dto)
        {
            await _thunderCqrs.SendCommand(new MigrateLearningProcessCommand
            {
                CourseIds = dto.CourseIds,
                BatchSize = dto.BatchSize
            });
        }
    }
}
