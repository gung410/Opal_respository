using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Controllers
{
    [Route("api/userTracking")]
    public class UserTrackingController : ApplicationApiController
    {
        private readonly IUserTrackingService _appService;

        public UserTrackingController(IUserContext userContext, IUserTrackingService appService) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost]
        public void SendEvent([ModelBinder(typeof(UserTrackingEventModelBinder))] UserTrackingEventRequest @event)
        {
            _appService.SendEvent(new UserActivityHappenedEvent(@event));
        }

        [HttpPost("trackingInfo/byItemId")]
        public Task<TrackingModel> GetTrackingByItemId([FromBody] GetTrackingByItemIdRequestDto request)
        {
            return _appService.GetTrackingByItemId(request, CurrentUserId);
        }

        [PermissionRequired(
            LearnerPermissionKeys.ActionCourseLikeShareCopy,
            LearnerPermissionKeys.ActionMicrolearningLikeShareCopy,
            LearnerPermissionKeys.ActionDigitalContentLikeShareCopy)]
        [HttpPost("like")]
        public Task<TrackingModel> Like([FromBody] UpdateTrackingRequestDto request)
        {
            return _appService.Like(request, CurrentUserId);
        }

        [PermissionRequired(
            LearnerPermissionKeys.ActionCourseLikeShareCopy,
            LearnerPermissionKeys.ActionMicrolearningLikeShareCopy,
            LearnerPermissionKeys.ActionDigitalContentLikeShareCopy)]
        [HttpPost("share")]
        public Task<TrackingModel> Share([FromBody] UpdateTrackingRequestDto request)
        {
            return _appService.Share(request, CurrentUserId);
        }

        [HttpGet("share/get")]
        public Task<PagedResultDto<TrackingSharedDetailByModel>> GetSharedToUserId(SearchTrackingSharedRequestDto request)
        {
            request.UserId = CurrentUserId;
            return _appService.GetSharedToUserId(request);
        }
    }
}
