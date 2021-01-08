using System;
using System.Threading.Tasks;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Controllers
{
    [Route("api/reviews")]
    public class UserReviewController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public UserReviewController(
            IUserContext userContext,
            IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpGet("")]
        public async Task<PagedReviewDto<UserReviewModel>> GetUserReviews(GetUserReviewRequest request)
        {
            var query = new GetUserReviewsQuery
            {
                ItemId = request.ItemId,
                ItemTypeFilter = request.ItemTypeFilter,
                OrderBy = request.OrderBy,
                ClassRunId = request.ClassRunId,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpGet("me")]
        public async Task<UserReviewModel> GetCurrentUserReview(GetUserReviewByCurrentUserRequest request)
        {
            var query = new GetUserReviewQuery
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType
            };

            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("create")]
        public async Task<UserReviewModel> CreateUserReview([FromBody] CreateUserReviewRequest request)
        {
            var userFullName = UserContext.GetValue<string>(CommonUserContextKeys.UserFullName);

            var command = new CreateUserReviewCommand
            {
                ItemId = request.ItemId,
                UserFullName = userFullName,
                Rating = request.Rating,
                CommentContent = request.CommentContent,
                ItemType = request.ItemType,
                ClassRunId = request.ClassRunId
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetUserReviewQuery
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPut("{itemId:guid}")]
        public async Task<UserReviewModel> UpdateUserReview(Guid itemId, [FromBody] UpdateUserReviewRequest request)
        {
            var command = new UpdateUserReviewCommand
            {
                Rating = request.Rating,
                CommentContent = request.CommentContent,
                ItemId = itemId,
                ItemType = request.ItemType
            };
            await _thunderCqrs.SendCommand(command);

            var query = new GetUserReviewQuery
            {
                ItemId = itemId,
                ItemType = request.ItemType
            };
            return await _thunderCqrs.SendQuery(query);
        }

        [HttpPost("triggerAverageRating")]
        public async Task TriggerAverageReviewRating([FromBody] MigrateAverageRatingRequestDto dto)
        {
            await _thunderCqrs.SendCommand(new MigrateAverageRatingCommand
            {
                DigitalContentIds = dto.DigitalContentIds,
                BatchSize = dto.BatchSize
            });
        }
    }
}
