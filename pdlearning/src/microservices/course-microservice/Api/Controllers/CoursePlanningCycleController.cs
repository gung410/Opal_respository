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
    [Route("api/coursePlanningCycle")]
    public class CoursePlanningCycleController : BaseController<CoursePlanningCycleService>
    {
        public CoursePlanningCycleController(IUserContext userContext, CoursePlanningCycleService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{coursePlanningCycleId:guid}")]
        public async Task<CoursePlanningCycleModel> GetCoursePlanningCycleById(Guid coursePlanningCycleId)
        {
            return await AppService.GetCoursePlanningCycleById(coursePlanningCycleId);
        }

        [HttpPost("getByIds")]
        public async Task<List<CoursePlanningCycleModel>> GetCoursePlanningCyclesByIds([FromBody] List<Guid> coursePlanningCycleIds)
        {
            return await AppService.GetCoursePlanningCyclesByIds(coursePlanningCycleIds);
        }

        [HttpPost("save")]
        public async Task<CoursePlanningCycleModel> SaveCoursePlanningCycle([FromBody] SaveCoursePlanningCycleRequest request)
        {
            return await AppService.SaveCoursePlanningCycle(request);
        }

        [HttpGet("search")]
        public async Task<PagedResultDto<CoursePlanningCycleModel>> SearchCoursePlanningCycle(SearchCoursesPlanningCyclesRequest request)
        {
            return await AppService.SearchCoursePlanningCycles(request);
        }
    }
}
