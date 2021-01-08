using System;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.RequestDtos;

namespace Microservice.Webinar.Application.Services.BigBlueButton
{
    public interface IBigBlueButtonService
    {
        Task<ResultCreateMeetingModel> CreateMeeting(CreateMeetingRequest request);

#pragma warning disable CA1055 // URI-like return values should not be strings
        string GetJoinUrl(JoinMeetingRequest request);
#pragma warning restore CA1055 // URI-like return values should not be strings

        bool ValidateMeetingTime(CheckMeetingAvailableRequest request);

        Task<bool> HasAttendeeRemainInMeeting(CheckMeetingAvailableRequest request);

        Task<BigBlueButtonMeetingInfoModel> GetMeetingInfo(Guid meetingId, string bbbServerPrivateIp);
    }
}
