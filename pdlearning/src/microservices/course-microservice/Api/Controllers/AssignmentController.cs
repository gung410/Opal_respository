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
    [Route("api/assignment")]
    public class AssignmentController : BaseController<AssignmentService>
    {
        public AssignmentController(IUserContext userContext, AssignmentService appService) : base(userContext, appService)
        {
        }

        [HttpGet("byId")]
        public async Task<AssignmentModel> GetAssignmentById(GetAssignmentByIdRequest request)
        {
            return await AppService.GetAssignmentById(request);
        }

        [HttpGet("getAssignments")]
        public async Task<PagedResultDto<AssignmentModel>> GetAssignments(GetAssignmentsRequest request)
        {
            return await AppService.GetAssignments(request);
        }

        [HttpGet("getNoOfAssignmentDones")]
        public async Task<IEnumerable<NoOfAssignmentDoneInfoModel>> GetNoOfAssignmentDones(GetNoOfAssignmentDonesRequest request)
        {
            return await AppService.GetNoOfAssignmentDones(request);
        }

        [HttpPost("save")]
        public async Task<AssignmentModel> SaveAssignment([FromBody] SaveAssignmentRequest request)
        {
            return await AppService.SaveAssignment(request);
        }

        [HttpPost("getAssignmentByIds")]
        public async Task<IEnumerable<AssignmentModel>> GetAssignmentByIds([FromBody] GetAssignmentsByIdsRequest request)
        {
            return await AppService.GetAssignmentByIds(request);
        }

        [HttpDelete("{assignmentId:guid}")]
        public async Task DeleteAssignment(Guid assignmentId)
        {
            await AppService.DeleteAssignment(assignmentId);
        }

        [HttpPost("setupPeerAssessment")]
        public async Task SetupPeerAssessment([FromBody] SetupPeerAssessmentRequest request)
        {
            await AppService.SetupPeerAssessment(request);
        }
    }
}
