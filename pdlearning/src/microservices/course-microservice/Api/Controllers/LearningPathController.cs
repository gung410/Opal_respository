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
    [Route("api/learningpaths")]
    public class LearningPathController : BaseController<LearningPathService>
    {
        public LearningPathController(IUserContext userContext, LearningPathService appService) : base(userContext, appService)
        {
        }

        [HttpGet("search")]
        public async Task<PagedResultDto<LearningPathModel>> SearchLearningPath(SearchLearningPathRequest request)
        {
            return await AppService.SearchLearningPaths(request);
        }

        [HttpGet("{learningPathId:guid}")]
        public async Task<LearningPathModel> GetLearningPathById(Guid learningPathId)
        {
            return await AppService.GetLearningPathById(learningPathId);
        }

        [HttpPost("getByIds")]
        public async Task<List<LearningPathModel>> GetLearningPathByIds([FromBody] Guid[] learningPathIds)
        {
            return await AppService.GetLearningPathByIds(learningPathIds);
        }

        [HttpPost("save")]
        public async Task<LearningPathModel> SaveLearningPathAsync([FromBody] SaveLearningPathRequest request)
        {
            return await AppService.SaveLearningPath(request);
        }
    }
}
