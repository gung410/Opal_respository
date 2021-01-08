using System;
using System.Threading.Tasks;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Services
{
    public interface IUserTrackingService
    {
        void SendEvent(UserActivityHappenedEvent @event);

        Task<TrackingModel> GetTrackingByItemId(GetTrackingByItemIdRequestDto request, Guid userId);

        Task<TrackingModel> Like(UpdateTrackingRequestDto request, Guid userId);

        Task<TrackingModel> Share(UpdateTrackingRequestDto request, Guid userId);

        Task<PagedResultDto<TrackingSharedDetailByModel>> GetSharedToUserId(SearchTrackingSharedRequestDto request);
    }
}
