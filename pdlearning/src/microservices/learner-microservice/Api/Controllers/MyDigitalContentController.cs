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
    [Route("api/me/digitalcontent")]
    public class MyDigitalContentController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyDigitalContentController(IThunderCqrs thunderCqrs, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [PermissionRequired(LearnerPermissionKeys.ViewDigitalContent)]
        [HttpPost("search")]
        public async Task<SearchPagedResultDto<DigitalContentModel, MyDigitalContentStatisticModel>> SearchMyDigitalContent(
            [FromBody] SearchMyDigitalContentRequestDto request)
        {
            var query = new SearchDigitalContentQuery
            {
                SearchText = request.SearchText,
                IncludeStatistic = request.IncludeStatistic,
                StatusFilter = request.StatusFilter,
                StatusStatistics = request.StatisticsFilter,
                DigitalContentType = request.DigitalContentType,
                OrderBy = request.OrderBy,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("getIds")]
        public async Task<List<DigitalContentModel>> GetMyDigitalContentsByIds([FromBody] GetMyDigitalContentByIdsRequest request)
        {
            var query = new GetMyDigitalContentByIdsQuery
            {
                DigitalContentIds = request.DigitalContentIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewDigitalContent)]
        [HttpGet("details/{digitalContentId:guid}")]
        public async Task<DigitalContentModel> GetDetailByDigitalContentId(Guid digitalContentId)
        {
            var query = new GetDetailByDigitalContentIdQuery
            {
                DigitalContentId = digitalContentId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPost("update")]
        public async Task<DigitalContentModel> UpdateMyDigitalContent([FromBody] UpdateMyDigitalContentRequest request)
        {
            var command = new UpdateMyDigitalContentCommand
            {
                DigitalContentId = request.DigitalContentId,
                ProgressMeasure = request.ProgressMeasure,
                ReadDate = request.ReadDate,
                Status = request.Status
            };

            await _thunderCqrs.SendCommand(command);

            var query = new GetDetailByDigitalContentIdQuery
            {
                DigitalContentId = request.DigitalContentId
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.StartLearning)]
        [HttpPost("enroll")]
        public async Task EnrollMyDigitalContent([FromBody] EnrollMyDigitalContentRequest request)
        {
            var command = new EnrollMyDigitalContentCommand
            {
                DigitalContentId = request.DigitalContentId
            };

            await _thunderCqrs.SendCommand(command);
        }

        [HttpPost("migrate")]
        public async Task<int> MigrateMyDigitalContent([FromBody] MigrateMyDigitalContentRequest request)
        {
            var query = new GetMyDigitalContentToMigrateQuery
            {
                BatchSize = request.BatchSize,
                MigrationEventType = request.MigrationEventType,
                OriginalObjectIds = request.OriginalObjectIds,
                Statuses = request.Statuses
            };

            return await _thunderCqrs.SendQuery(query);
        }
    }
}
