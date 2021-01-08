using System.Threading.Tasks;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Controllers
{
    // Use 'me' keyword like OneDrive REST API
    // https://docs.microsoft.com/en-us/onedrive/developer/rest-api/api/drive_get?view=odsp-graph-online
    [Route("api/me/learning")]
    public class MyLearningPackageController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyLearningPackageController(IUserContext userContext, IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpGet("getLearningPackage")]
        public async Task<MyLearningPackageModel> GetMyLearningPackageByMyLectureId(GetMyLearningPackageRequestDto dto)
        {
            var query = new GetMyLearningPackageQuery
            {
                MyLectureId = dto.MyLectureId,
                MyDigitalContentId = dto.MyDigitalContentId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("createOrUpdateLearningPackage")]
        public async Task<MyLearningPackageModel> CreateMyLearningPackage([FromBody] CreateOrUpdateMyLearningPackageRequestDto dto)
        {
            var command = new CreateOrUpdateMyLearningPackageCommand
            {
                MyLectureId = dto.MyLectureId,
                MyDigitalContentId = dto.MyDigitalContentId,
                Type = dto.Type,
                State = dto.State != null ? System.Text.Json.JsonSerializer.Serialize(dto.State) : null,
                LessonStatus = dto.LessonStatus,
                CompletionStatus = dto.CompletionStatus,
                SuccessStatus = dto.SuccessStatus,
                TimeSpan = dto.TimeSpan
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetMyLearningPackageQuery
            {
                MyLectureId = dto.MyLectureId,
                MyDigitalContentId = dto.MyDigitalContentId
            };
            return await _thunderCqrs.SendQuery(query);
        }
    }
}
