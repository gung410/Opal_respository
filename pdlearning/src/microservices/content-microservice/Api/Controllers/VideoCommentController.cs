using System;
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
    [Route("api/content/videocomment")]
    public class VideoCommentController : ApplicationApiController
    {
        private readonly IVideoCommentService _appService;

        public VideoCommentController(IVideoCommentService appService, IUserContext userContext) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost("create")]
        public async Task<VideoCommentModel> CreateVideoComment([FromBody] CreateVideoCommentRequest request)
        {
            return await _appService.CreateVideoComment(request, CurrentUserId);
        }

        [HttpPost("update")]
        public async Task<VideoCommentModel> UpdateVideoComment([FromBody] UpdateVideoCommentRequest request)
        {
            return await _appService.UpdateVideoComment(request, CurrentUserId);
        }

        [HttpDelete("{videoCommentId:guid}")]
        public async Task DeleteVideoComment(Guid videoCommentId)
        {
            await _appService.DeleteVideoComment(videoCommentId, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<VideoCommentModel>> SearchVideoComments([FromBody] SearchVideoCommentRequest request)
        {
            return await _appService.SearchVideoComments(request, CurrentUserId);
        }
    }
}
