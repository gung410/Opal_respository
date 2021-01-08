using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Controllers
{
    [Route("api/form/responses")]
    public class ResponsesController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public ResponsesController(IThunderCqrs thunderCqrs, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpPost("getAll")]
        public Task<PagedResultDto<ResponsesModel>> GetAll(GetAllResponsesRequest request)
        {
            return _thunderCqrs.SendQuery(new GetAllResponseQuery { PagedInfo = request.PagedInfo });
        }

        [HttpPost]
        public Task Update(UpdateResponseRequest request)
        {
            return _thunderCqrs.SendCommand(new UpdateSurveyResponseStatusCommand(
                request.FormId,
                request.UserId,
                request.AttendanceTime,
                request.SubmittedTime));
        }
    }
}
