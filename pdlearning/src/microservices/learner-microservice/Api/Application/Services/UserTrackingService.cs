using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Services
{
    public class UserTrackingService : ApplicationService, IUserTrackingService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public UserTrackingService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public void SendEvent(UserActivityHappenedEvent @event)
        {
            _thunderCqrs.SendEvent(@event);
        }

        public Task<TrackingModel> GetTrackingByItemId(GetTrackingByItemIdRequestDto request, Guid userId)
        {
            var query = new GetTrackingByItemIdQuery
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType
            };

            return _thunderCqrs.SendQuery(query);
        }

        public async Task<TrackingModel> Like(UpdateTrackingRequestDto request, Guid userId)
        {
            if (!request.IsLike.HasValue)
            {
                throw new BusinessLogicException("Like feature get no data!");
            }

            var command = new UpdateTrackingCommand
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType,
                TrackingAction = LearningTrackingAction.Like,
                IsLike = request.IsLike.Value,
            };
            await this._thunderCqrs.SendCommand(command);

            var query = new GetTrackingByItemIdQuery
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType,
                TrackingAction = LearningTrackingAction.Like
            };
            return await _thunderCqrs.SendQuery(query);
        }

        public async Task<TrackingModel> Share(UpdateTrackingRequestDto request, Guid userId)
        {
            if (request.SharedUsers == null || request.SharedUsers.Count == 0)
            {
                throw new BusinessLogicException("Sharing feature get no one!");
            }

            var command = new CreateUserSharingCommand
            {
                ItemType = (SharingType)request.ItemType,
                ItemId = request.ItemId,
                UsersShared = request.SharedUsers
                    .Select(sharedUser => new SaveUserSharingDetailRequestDto
                    {
                        UserId = sharedUser
                    })
                    .ToList()
            };

            await _thunderCqrs.SendCommand(command);

            var query = new GetTrackingByItemIdQuery
            {
                ItemId = request.ItemId,
                ItemType = request.ItemType,
                TrackingAction = LearningTrackingAction.Share
            };
            return await _thunderCqrs.SendQuery(query);
        }

        public Task<PagedResultDto<TrackingSharedDetailByModel>> GetSharedToUserId(SearchTrackingSharedRequestDto request)
        {
            var searchQuery = new SearchTrackingSharedQuery
            {
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return _thunderCqrs.SendQuery(searchQuery);
        }
    }
}
