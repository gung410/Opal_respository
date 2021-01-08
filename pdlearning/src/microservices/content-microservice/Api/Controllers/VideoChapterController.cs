using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Controllers
{
    [Route("api/content/videochapter")]
    public class VideoChapterController : ApplicationApiController
    {
        private readonly IVideoChapterService _appService;

        public VideoChapterController(IVideoChapterService appService, IUserContext userContext) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost("create")]
        public async Task<List<ChapterModel>> CreateVideoChapter([FromBody] CreateVideoChapterRequest request)
        {
            return await _appService.CreateVideoChapter(request, CurrentUserId);
        }

        [HttpPost("update")]
        public async Task<List<ChapterModel>> UpdateVideoChapter([FromBody] UpdateVideoChapterRequest request)
        {
            return await _appService.UpdateVideoChapter(request, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<List<ChapterModel>> SearchVideoChapters([FromBody] SearchVideoChapterRequest request)
        {
            return await _appService.SearchVideoChapters(request, CurrentUserId);
        }
    }
}
