using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Controllers
{
    [Route("api/me/assignments")]
    public class MyAssignmentController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<MyAssignmentController> _logger;

        public MyAssignmentController(
            IUserContext userContext,
            IThunderCqrs thunderCqrs,
            ILogger<MyAssignmentController> logger) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<PagedResultDto<MyAssignmentModel>> GetMyAssignmentByCurrentUser(GetAssignmentRequest request)
        {
            var query = new GetAssignmentByCurrentUserQuery
            {
                RegistrationId = request.RegistrationId,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("changeStatus")]
        public async Task ChangeAssignmentStatus([FromBody] ChangeAssignmentStatusRequest request)
        {
            // Change assignment status to completed was unexpected to do by frontend.
            if (request.Status == MyAssignmentStatus.Completed)
            {
                _logger.LogWarning("Change assignment status to completed was unexpected to do by frontend!");
                return;
            }

            var command = new ChangeAssignmentStatusCommand
            {
                Status = request.Status,
                AssignmentId = request.AssignmentId,
                RegistrationId = request.RegistrationId
            };

            await _thunderCqrs.SendCommand(command);
        }

        [HttpPost("byAssignmentIds")]
        public async Task<List<MyAssignmentModel>> GetMyAssignmentByAssignmentId([FromBody] List<Guid> assignmentIds)
        {
            var query = new GetMyAssignmentsByAssignmentIdsQuery
            {
                AssignmentIds = assignmentIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpGet("byAssignmentId/{assignmentId}")]
        public async Task<MyAssignmentModel> GetMyAssignmentByAssignmentId(Guid assignmentId)
        {
            var query = new GetMyAssignmentsByAssignmentIdQuery
            {
                AssignmentId = assignmentId
            };

            return await _thunderCqrs.SendQuery(query);
        }
    }
}
