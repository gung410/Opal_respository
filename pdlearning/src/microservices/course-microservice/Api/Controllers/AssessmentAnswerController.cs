using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/assessmentAnswer")]
    public class AssessmentAnswerController : BaseController<AssessmentAnswerService>
    {
        public AssessmentAnswerController(IUserContext userContext, AssessmentAnswerService appService) : base(userContext, appService)
        {
        }

        [HttpGet("byIdOrUser")]
        public async Task<AssessmentAnswerModel> GetAssessmentAnswerByIdOrUser(GetAssessmentAnswerByIdOrUserRequest request)
        {
            return await AppService.GetAssessmentAnswerByIdOrUser(request);
        }

        [HttpPost("save")]

        public async Task<AssessmentAnswerModel> SaveAssessmentAnswer([FromBody] SaveAssessmentAnswerRequest request)
        {
            return await AppService.SaveAssessmentAnswer(request);
        }

        [HttpGet("getNoOfAssessmentDones")]
        public async Task<IEnumerable<NoOfAssessmentDoneInfoModel>> GetNoOfAssessmentDones(GetNoOfAssessmentDonesRequest request)
        {
            return await AppService.GetNoOfAssessmentDones(request);
        }

        [HttpPost("createAssessmentAnswer")]
        public async Task<AssessmentAnswerModel> CreatePeerAssessor([FromBody] CreateAssessmentAnswerRequest request)
        {
            return await AppService.CreateAssessmentAnswer(request);
        }

        [HttpDelete("{id:guid}")]
        public async Task DeleteAssessmentAnswer(Guid id)
        {
            await AppService.DeleteAssessmentAnswer(id);
        }

        [HttpPost("searchAssessmentAnswer")]
        public async Task<PagedResultDto<AssessmentAnswerModel>> SearchAssessmentAnswer([FromBody] SearchAssessmentAnswerRequest request)
        {
            return await AppService.SearchAssessmentAnswer(request);
        }
    }
}
