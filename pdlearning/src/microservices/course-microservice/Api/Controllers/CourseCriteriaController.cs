using System;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/courseCriteria")]
    public class CourseCriteriaController : BaseController<CourseCriteriaService>
    {
        public CourseCriteriaController(IUserContext userContext, CourseCriteriaService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{courseCriteriaId:guid}")]
        public async Task<CourseCriteriaModel> GetCourseCriteriaByIdQuery(Guid courseCriteriaId)
        {
            return await AppService.GetCourseCriteriaByIdQuery(courseCriteriaId);
        }

        [HttpPost("save")]
        public async Task<CourseCriteriaModel> SaveCourseCriteria([FromBody] SaveCourseCriteriaRequest request)
        {
            return await AppService.SaveCourseCriteria(request);
        }
    }
}
