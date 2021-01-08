using System;
using System.Threading.Tasks;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Controllers
{
    [Route("api/me/outstandingTasks")]
    public class MyOutstandingTaskController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyOutstandingTaskController(
            IUserContext userContext,
            IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpGet]
        public async Task<PagedResultDto<OutstandingTaskModel>> GetMyOutstandingTasks(GetMyOutstandingTaskRequestDto request)
        {
            var query = new GetMyOutstandingTasksQuery
            {
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpGet("byId/{id:guid}")]
        public async Task<OutstandingTaskModel> GetMyOutstandingTasks(Guid id)
        {
            var query = new GetMyOutstandingTaskByIdQuery
            {
                Id = id
            };

            return await _thunderCqrs.SendQuery(query);
        }
    }
}
