using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.RequestDtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Timing;

namespace Microservice.Webinar.Application.Services.BigBlueButton
{
    public class BigBlueButtonService : IBigBlueButtonService, IApplicationService
    {
        public const string SuccessStatusCode = "SUCCESS";
        public const string FailedStatusCode = "FAILED";

        private readonly BigBlueButtonServerOptions _serverInfo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUrlMeetingBuilder _urlBuilder;
        private readonly ILogger<BigBlueButtonService> _logger;

        public BigBlueButtonService(
            IHttpClientFactory httpClientFactory,
            IOptions<BigBlueButtonServerOptions> bigBlueButtonServer,
            IUrlMeetingBuilder urlBuilder,
            ILogger<BigBlueButtonService> logger)
        {
            _serverInfo = bigBlueButtonServer.Value;
            _httpClientFactory = httpClientFactory;
            _urlBuilder = urlBuilder;
            _logger = logger;
        }

        public bool ValidateMeetingTime(CheckMeetingAvailableRequest request)
        {
            // the start time valid is time that before 30 mins of the meeting.
            var startTimeValid = request.StartTime.AddMinutes(-30);

            // the end time valid is time that no greater than 30 mins from the meeting end time.
            var endTimeValid = request.EndTime.AddMinutes(30);

            // Valid if the current time is in the valid range:
            // startTimeValid <= currentTime <= endTimeValid
            return (DateTime.Compare(startTimeValid, Clock.Now) <= 0) && (DateTime.Compare(Clock.Now, endTimeValid) <= 0);
        }

        public async Task<bool> HasAttendeeRemainInMeeting(CheckMeetingAvailableRequest request)
        {
            // the end time valid is time that no greater than 30 mins from the meeting end time.
            var endTimeValid = request.EndTime.AddMinutes(30);

            // In case current time was pass the meeting end time
            // It still valid if there are any attendee remains in the meeting
            if (DateTime.Compare(Clock.Now, endTimeValid) <= 0)
            {
                return false;
            }

            var meetingInfo = await GetMeetingInfo(request.MeetingId, request.BBBServerPrivateIp);
            return meetingInfo != null && meetingInfo.ParticipantCount > 0;
        }

        public async Task<ResultCreateMeetingModel> CreateMeeting(CreateMeetingRequest request)
        {
            var result = new ResultCreateMeetingModel();

            var httpClient = _httpClientFactory.CreateClient();

            var apiUrl = _urlBuilder.BuildCreateMeetingApiUrl(
                new CreateMeetingUrlParams(
                    meetingId: request.MeetingId.ToString(),
                    attendeePwd: _serverInfo.AttendeePassword,
                    moderatorPwd: _serverInfo.ModeratorPassword,
                    meetingName: request.MeetingName,
                    secretKey: _serverInfo.BBBSecretKey,
                    baseApiUrl: _serverInfo.WebinarUrl,
                    bbbServerPrivateIp: request.BBBServerPrivateIp,
                    logoutUrl: _serverInfo.LogoutUrl));

            // TODO: Remove after tests.
            _logger.LogWarning("5dw48sd6 debug get join link:" + apiUrl);
            var response = await httpClient.GetAsync(apiUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(responseContent);
                }
                catch (XmlException ex)
                {
                    _logger.LogError(ex, "Failed to load XML. ResponseContent: {ResponseContent}", responseContent);
                    result.IsSuccess = false;
                    result.Message = "Could not create the meeting.";
                    result.MessageCode = "COULD_NOT_CREATE_MEETING";

                    return result;
                }

                var statusCode = GetXmlNodeContent(xmlDocument, "/response/returncode");
                result.MessageCode = GetXmlNodeContent(xmlDocument, "/response/messageKey");
                result.Message = GetXmlNodeContent(xmlDocument, "/response/message");
                result.IsSuccess = statusCode == SuccessStatusCode;

                _logger.LogInformation(
                    "[CreateMeeting] {Status} - {MessageCode}: {Message} ",
                    statusCode,
                    result.MessageCode,
                    result.Message);
            }

            return result;
        }

#pragma warning disable CA1055 // URI-like return values should not be strings
        public string GetJoinUrl(JoinMeetingRequest request)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            var apiUrl = _urlBuilder.BuildJoinUrl(
                new CreateJoinUrlParams(
                    userId: request.UserId,
                    fullName: request.FullName,
                    meetingId: request.MeetingId,
                    isModerator: request.IsModerator,
                    moderatorPwd: _serverInfo.ModeratorPassword,
                    attendeePwd: _serverInfo.AttendeePassword,
                    redirect: request.Redirect,
                    secretKey: _serverInfo.BBBSecretKey,
                    proxySecretKey: _serverInfo.ProxySecretKey,
                    baseApiUrl: _serverInfo.WebinarUrl,
                    baseProxyUrl: _serverInfo.ProxyUrl,
                    bbbServerPrivateIp: request.BBBServerPrivateIp));

            return apiUrl;
        }

        public async Task<BigBlueButtonMeetingInfoModel> GetMeetingInfo(Guid meetingId, string bbbServerPrivateIp)
        {
            var result = new BigBlueButtonMeetingInfoModel();

            var httpClient = _httpClientFactory.CreateClient();

            var apiUrl = _urlBuilder.BuildGetMeetingInfoApiUrl(
                new GetMeetingInfoUrlParams(
                    meetingId: meetingId,
                    secretKey: _serverInfo.BBBSecretKey,
                    baseApiUrl: _serverInfo.WebinarUrl,
                    bbbServerPrivateIp: bbbServerPrivateIp));

            var response = await httpClient.GetAsync(apiUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(responseContent);
                }
                catch (XmlException ex)
                {
                    _logger.LogError(ex, "Failed to load XML. ResponseContent: {ResponseContent}", responseContent);

                    return null;
                }

                var statusCode = GetXmlNodeContent(xmlDocument, "/response/returncode");

                if (statusCode == SuccessStatusCode)
                {
                    result.ParticipantCount = int.Parse(GetXmlNodeContent(xmlDocument, "/response/participantCount"));
                }
                else
                {
                    _logger.LogInformation(
                        "[CreateMeeting] {Status} - {MessageKey}: {Message} ",
                        statusCode,
                        GetXmlNodeContent(xmlDocument, "/response/messageKey"),
                        GetXmlNodeContent(xmlDocument, "/response/message"));
                }
            }

            return result;
        }

        private string GetXmlNodeContent(XmlDocument document, string nodeKey)
        {
            var node = document.SelectSingleNode(nodeKey);

            return node != null ? node.InnerText : string.Empty;
        }
    }
}
