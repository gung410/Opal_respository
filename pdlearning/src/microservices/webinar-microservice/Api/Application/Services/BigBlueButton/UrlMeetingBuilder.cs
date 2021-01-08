using System.Collections.Generic;
using System.Web;
using Microservice.Webinar.Common.Extensions;
using Thunder.Platform.Core.Helpers;

namespace Microservice.Webinar.Application.Services.BigBlueButton
{
#pragma warning disable CA1055 // URI-like return values should not be strings
    public class UrlMeetingBuilder : IUrlMeetingBuilder
    {
        private const string CreateMeetingActionName = "create";
        private const string JoinMeetingActionName = "join";
        private const string GetMeetingInfoActionName = "getMeetingInfo";

        /// <inheritdoc/>
        public string BuildCreateMeetingApiUrl(CreateMeetingUrlParams @params)
        {
            var basicQueryParams = new List<string>
            {
                $"meetingID={@params.MeetingId}",
                $"attendeePW={HttpUtility.UrlEncode(@params.AttendeePassword)}",
                $"moderatorPW={HttpUtility.UrlEncode(@params.ModeratorPassword)}",
                $"name={HttpUtility.UrlEncode(@params.MeetingName)}",
                $"welcome={HttpUtility.UrlEncode($"<br>Welcome to <b>{@params.MeetingName}</b>!")}",
                $"logoutURL={@params.LogoutUrl}",
                "record=true",
                "allowStartStopRecording=true"
            };

            // This getting URL being like:  https://webinar.development.net/bbb-xxx.
            var baseAreaBBBServerAddress = GetBaseAreaBBBServerAddress(@params.BaseApiUrl, @params.BBBServerPrivateIp);
            return BuildApiUrl(
                CreateMeetingActionName, basicQueryParams, @params.SecretKey, baseAreaBBBServerAddress);
        }

        /// <inheritdoc/>
        public string BuildGetMeetingInfoApiUrl(GetMeetingInfoUrlParams @params)
        {
            var basicQueryParams = new List<string>
            {
                 $"meetingID={@params.MeetingId}"
            };

            // This getting URL being like:  https://webinar.development.net/bbb-xxx.
            var baseAreaBBBServerAddress = GetBaseAreaBBBServerAddress(@params.BaseApiUrl, @params.BBBServerPrivateIp);
            return BuildApiUrl(
                GetMeetingInfoActionName, basicQueryParams, @params.SecretKey, baseAreaBBBServerAddress);
        }

        /// <inheritdoc/>
        public string BuildJoinUrl(CreateJoinUrlParams @params)
        {
            var basicQueryParams = new List<string>
            {
                $"area={HttpUtility.UrlEncode(@params.BBBServerPrivateIp.ConvertIpAddressToPattern())}",
                $"fullName={HttpUtility.UrlEncode(@params.FullName)}",
                $"meetingID={@params.MeetingId}",
                $"password={HttpUtility.UrlEncode(@params.GetPassword())}",
                $"redirect={HttpUtility.UrlEncode(@params.Redirect.ToString().ToLower())}",
                $"userID={@params.UserId}"
            };

            return BuildApiUrl(
                JoinMeetingActionName, basicQueryParams, @params.ProxySecretKey, @params.BaseProxyUrl);
        }

        private string BuildApiUrl(string actionName, List<string> queryParams, string secretKey, string baseApiUrl)
        {
            var queryUrl = $"{string.Join("&", queryParams)}";

            // Combine the url with action name and secret key.
            var queryUrlWithSecretKey = $"{actionName}{queryUrl}{secretKey}";

            // Create a checksum for the query.
            var checkSum = EncryptionHelper.ComputeSha1Hash(queryUrlWithSecretKey);

            // Append the checksum to the query params.
            queryUrl = $"{queryUrl}&checksum={checkSum}";

            // example: https://webinar.opal.edu.sg/bigbluebutton/create?fullName=....
            var bigBlueButtonApiUrl = $"{baseApiUrl}/{actionName}?{queryUrl}";

            return bigBlueButtonApiUrl;
        }

        private string GetBaseAreaBBBServerAddress(string baseApiUrl, string bbbServerPrivateIp)
        {
            return $"{baseApiUrl}/bbb-{bbbServerPrivateIp.ConvertIpAddressToPattern()}/bigbluebutton/api";
        }
    }
#pragma warning restore CA1055 // URI-like return values should not be strings
}
