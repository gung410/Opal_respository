using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Controllers
{
    [Route("api/form/comment")]
    public class CommentController : BaseController<CommentApplicationService>
    {
        public CommentController(CommentApplicationService appService, IUserContext userContext) : base(userContext, appService)
        {
        }

        [HttpPost("create")]
        public async Task<CommentModel> CreateComment([FromBody] CreateCommentRequest request)
        {
            return await AppService.CreateComment(request, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<CommentModel>> SearchComments([FromBody] SearchCommentRequest request)
        {
            return await AppService.SearchComments(request);
        }
    }
}
