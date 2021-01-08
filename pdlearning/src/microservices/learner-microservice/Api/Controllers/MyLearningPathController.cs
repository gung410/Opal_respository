using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/me/learningpaths")]
    public class MyLearningPathController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyLearningPathController(
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [PermissionRequired(LearnerPermissionKeys.ViewLearningPath)]
        [HttpPost("search")]
        public async Task<SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>> SearchMyLearningPath(
            [FromBody] SearchMyLearningPathRequestDto request)
        {
            var query = new SearchLearnerLearningPathQuery
            {
                SearchText = request.SearchText,
                IncludeStatistic = request.IncludeStatistic,
                LearningPathStatistic = request.StatisticsFilter,
                LearningPathType = request.StatusFilter,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("search/ids")]
        public async Task<List<LearnerLearningPathModel>> GetMyLearningPathByIds([FromBody] Guid[] ids)
        {
            var query = new GetLearnerLearningPathByIdsQuery
            {
                Ids = ids
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewLearningPath)]
        [HttpGet("detail/{id}")]
        public async Task<LearnerLearningPathModel> GetMyLearningPathById(Guid id)
        {
            var query = new GetLearnerLearningPathByIdQuery
            {
                Id = id
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpPost]
        public async Task<LearnerLearningPathModel> CreateMyLearningPath([FromBody] SaveLearnerLearningPathRequestDto request)
        {
            var command = new CreateLearnerLearningPathCommand
            {
                ThumbnailUrl = request.ThumbnailUrl,
                Title = request.Title,
                Courses = request.Courses
                    .Select(c => new SaveLearnerLearningPathCourseRequestDto
                    {
                        CourseId = c.CourseId,
                        Order = c.Order
                    })
                    .ToList()
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetLearnerLearningPathByIdQuery
            {
                Id = command.Id
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpPut]
        public async Task<LearnerLearningPathModel> UpdateMyLearningPath([FromBody] SaveLearnerLearningPathRequestDto request)
        {
            // For update case, the request Id must have value.
            if (!request.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var command = new UpdateLearnerLearningPathCommand(request.Id.Value)
            {
                ThumbnailUrl = request.ThumbnailUrl,
                Title = request.Title,
                Courses = request.Courses.Select(c => new SaveLearnerLearningPathCourseRequestDto
                {
                    CourseId = c.CourseId,
                    Order = c.Order
                }).ToList()
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetLearnerLearningPathByIdQuery
            {
                Id = command.Id
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpDelete("{id}")]
        public async Task DeleteMyLearningPath(Guid id)
        {
            var command = new DeleteLearnerLearningPathCommand
            {
                Id = id
            };

            await _thunderCqrs.SendCommand(command);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath, LearnerPermissionKeys.ViewLearningPath)]
        [HttpGet("searchUsers")]
        public async Task<PagedResultDto<UserModel>> SearchUsersForLearningPathSharing(SearchUsersForLearningPathRequestDto dto)
        {
            var query = new GetUsersQuery
            {
                SearchText = dto.SearchText,
                IncludeSubDepartments = true,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = dto.MaxResultCount,
                    SkipCount = dto.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpPost("enablePublic/{id:guid}")]
        public async Task EnablePublicLearningPath(Guid id)
        {
            var command = new EnablePublicLearningPathCommand
            {
                Id = id
            };

            await _thunderCqrs.SendCommand(command);
        }
    }
}
