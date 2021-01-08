using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Controllers
{
    [Route("api/content/comment")]
    public class CommentController : ApplicationApiController
    {
        private readonly ICommentService _appService;

        public CommentController(ICommentService appService, IUserContext userContext) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost("create")]
        public async Task<CommentModel> CreateComment([FromBody] CreateCommentRequest request)
        {
            return await _appService.CreateComment(request, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<CommentModel>> SearchComments([FromBody] SearchCommentRequest request)
        {
            return await _appService.SearchComments(request);
        }
    }
}
