using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Controllers
{
    [Route("api/userSharing")]
    public class UserSharingController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public UserSharingController(IThunderCqrs thunderCqrs, IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [PermissionRequired(LearnerPermissionKeys.ViewLearningPath)]
        [HttpGet("search")]
        public async Task<PagedResultDto<UserSharingModel>> GetUserSharing(GetUserSharingRequestDto request)
        {
            var query = new GetUserSharingQuery
            {
                SearchText = request.SearchText,
                ItemType = SharingType.LearningPath,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewLearningPath)]
        [HttpGet("shared/me/learningpath")]
        public async Task<PagedResultDto<LearnerLearningPathModel>> GetUserSharingLearningPathForMe(GetUserSharingRequestDto request)
        {
            var query = new GetUserSharingLearningPathForMeQuery
            {
                SearchText = request.SearchText,
                ItemType = SharingType.LearningPath,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("search/byItemIds")]
        public async Task<List<UserSharingModel>> GetUserSharingByItemIds([FromBody] Guid[] itemIds)
        {
            var query = new GetUserSharingByItemIdsQuery
            {
                ItemIds = itemIds
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.ViewLearningPath)]
        [HttpGet("details/byItemId/{itemId:guid}")]
        public async Task<UserSharingModel> GetUserSharingByItemId(Guid itemId)
        {
            var query = new GetUserSharingByItemIdQuery
            {
                ItemId = itemId
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpPost]
        public async Task<UserSharingModel> CreateUserSharing([FromBody] SaveUserSharingRequestDto request)
        {
            var command = new CreateUserSharingCommand
            {
                ItemType = request.ItemType,
                ItemId = request.ItemId,
                UsersShared = request.UsersShared
                    .Select(c => new SaveUserSharingDetailRequestDto
                    {
                        UserId = c.UserId
                    })
                    .ToList()
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetUserSharingByIdQuery
            {
                Id = command.Id
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpPut]
        public async Task<UserSharingModel> UpdateUserSharing([FromBody] SaveUserSharingRequestDto request)
        {
            // For update case, the request Id must have value.
            if (!request.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var command = new UpdateUserSharingCommand(request.Id.Value)
            {
                UsersShared = request.UsersShared
                    .Select(c => new SaveUserSharingDetailRequestDto
                    {
                        UserId = c.UserId
                    })
                    .ToList()
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetUserSharingByIdQuery
            {
                Id = command.Id
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [PermissionRequired(LearnerPermissionKeys.CudLearningPath)]
        [HttpDelete("{id:guid}")]
        public async Task DeleteUserSharing(Guid id)
        {
            var command = new DeleteUserSharingCommand
            {
                Id = id
            };

            await _thunderCqrs.SendCommand(command);
        }
    }
}
