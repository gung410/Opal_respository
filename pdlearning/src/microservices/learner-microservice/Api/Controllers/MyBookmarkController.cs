using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Controllers
{
    [Route("api/me/bookmarks")]
    public class MyBookmarkController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public MyBookmarkController(IThunderCqrs thunderCqrs, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [PermissionRequired(LearnerPermissionKeys.ViewUserBookmark)]
        [HttpGet("")]
        public async Task<PagedResultDto<UserBookmarkModel>> GetMyBookmarkByType(GetMyBookmarkRequest request)
        {
            var query = new GetMyBookmarkByTypeQuery
            {
                ItemType = request.ItemType,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpGet("ids")]
        public async Task<List<UserBookmarkModel>> GetMyBookmarkByItemIds(GetMyBookmarkByItemIdsRequestDto request)
        {
            var query = new GetMyBookmarkByItemIdsQuery
            {
                ItemType = request.ItemType,
                ItemIds = request.ItemIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpGet("countuserbookmark")]
        public async Task<List<UserBookmarkedModel>> GetCountUserBookmarked(GetCountUserBookmarkedRequestDto request)
        {
            var query = new GetCountUserBookmarkedQuery
            {
                ItemType = request.ItemType,
                ItemIds = request.ItemIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewUserBookmark)]
        [HttpGet("digitalcontent")]
        public async Task<PagedResultDto<DigitalContentModel>> GetMyDigitalContentBookmarks(GetMyBookmarkRequest request)
        {
            var query = new GetMyDigitalContentBookmarkQuery
            {
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewUserBookmark)]
        [HttpGet("learningpath")]
        public async Task<PagedResultDto<LearnerLearningPathModel>> GetMyLearningPathBookmarks(GetMyBookmarkRequest request)
        {
            var query = new GetMyLearningPathBookmarkQuery
            {
                ItemType = request.ItemType,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ActionBookmark)]
        [HttpPost("create")]
        public async Task<UserBookmarkModel> CreateMyBookmark([FromBody] CreateMyBookmarkRequest request)
        {
            var command = new CreateUserBookmarkCommand
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetMyBookmarkByIdQuery
            {
                Id = command.Id
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ActionBookmark)]
        [HttpDelete("unbookmark/{id:guid}")]
        public async Task UnBookmarkById(Guid id)
        {
            var command = new DeleteBookmarkByIdCommand
            {
                Id = id
            };

            await _thunderCqrs.SendCommand(command);
        }

        [PermissionRequired(LearnerPermissionKeys.ActionBookmark)]
        [HttpDelete("unbookmarkItem/{itemId:guid}")]
        public async Task UnBookmarkByItemId(Guid itemId)
        {
            var command = new DeleteBookmarkByItemIdCommand
            {
                ItemId = itemId
            };

            await _thunderCqrs.SendCommand(command);
        }
    }
}
