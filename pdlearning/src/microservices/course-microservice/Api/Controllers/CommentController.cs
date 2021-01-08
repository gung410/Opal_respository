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
    [Route("api/course/comment")]
    public class CommentController : BaseController<CommentService>
    {
        public CommentController(CommentService appService, IUserContext userContext) : base(userContext, appService)
        {
        }

        [HttpPost("create")]
        public async Task<CommentModel> CreateComment([FromBody] CreateCommentRequest request)
        {
            return await AppService.CreateComment(request);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<CommentModel>> SearchComments([FromBody] SearchCommentRequest request)
        {
            return await AppService.SearchComments(request);
        }

        [HttpPost("getCommentNotSeen")]
        public async Task<IEnumerable<SeenCommentModel>> GetCommentNotSeen([FromBody] GetCommentNotSeenRequest request)
        {
            return await AppService.GetCommentNotSeen(request);
        }
    }
}
