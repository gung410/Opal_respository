using System;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Webinar.Application.Services.BigBlueButton
{
#pragma warning disable CA1055 // URI-like return values should not be strings
    public interface IUrlMeetingBuilder
    {
        /// <summary>
        /// To build the BigBlueButton API URL for the meeting creation.
        /// </summary>
        /// <param name="params">The required params to build the url.</param>
        /// <returns>The API URL for the meeting creation.</returns>
        string BuildCreateMeetingApiUrl(CreateMeetingUrlParams @params);

        /// <summary>
        /// To build the join url for the meeting.
        /// </summary>
        /// <param name="params">The required params to build the url.</param>
        /// <returns>The join url.</returns>
        string BuildJoinUrl(CreateJoinUrlParams @params);

        /// <summary>
        /// To build the BigBlueButton API URL for get meeting information.
        /// </summary>
        /// <param name="params">The required params to build the url.</param>
        /// <returns>The API URL for the get meeting information.</returns>
        string BuildGetMeetingInfoApiUrl(GetMeetingInfoUrlParams @params);
    }
#pragma warning restore CA1055 // URI-like return values should not be strings

    public class CreateMeetingUrlParams
    {
        public CreateMeetingUrlParams(
            string meetingId,
            string attendeePwd,
            string moderatorPwd,
            string meetingName,
            string secretKey,
            string baseApiUrl,
            string bbbServerPrivateIp,
            string logoutUrl)
        {
            MeetingId = meetingId;
            AttendeePassword = attendeePwd;
            ModeratorPassword = moderatorPwd;
            MeetingName = meetingName;
            SecretKey = secretKey;
            BaseApiUrl = baseApiUrl;
            BBBServerPrivateIp = bbbServerPrivateIp;
            LogoutUrl = logoutUrl;
        }

        public string MeetingId { get; }

        public string AttendeePassword { get; }

        public string ModeratorPassword { get; }

        public string MeetingName { get; }

        public string SecretKey { get; }

        public string BaseApiUrl { get; }

        public string BaseProxyApiUrl { get; }

        public string BBBServerPrivateIp { get; }

        public string LogoutUrl { get; }
    }

    public class CreateJoinUrlParams
    {
        public CreateJoinUrlParams(
            Guid userId,
            string fullName,
            string meetingId,
            bool isModerator,
            string moderatorPwd,
            string attendeePwd,
            bool redirect,
            string secretKey,
            string proxySecretKey,
            string baseApiUrl,
            string baseProxyUrl,
            string bbbServerPrivateIp)
        {
            UserId = userId;
            FullName = fullName;
            MeetingId = meetingId;
            IsModerator = isModerator;
            ModeratorPassword = moderatorPwd;
            AttendeePassword = attendeePwd;
            Redirect = redirect;
            SecretKey = secretKey;
            ProxySecretKey = proxySecretKey;
            BaseApiUrl = baseApiUrl;
            BaseProxyUrl = baseProxyUrl;
            BBBServerPrivateIp = bbbServerPrivateIp;
        }

        public Guid UserId { get; }

        public string FullName { get; }

        public string MeetingId { get; }

        public bool IsModerator { get; }

        public string ModeratorPassword { get; }

        public string AttendeePassword { get; }

        public bool Redirect { get; }

        public string SecretKey { get; }

        public string ProxySecretKey { get; }

        public string BaseApiUrl { get; }

        public string BaseProxyUrl { get; set; }

        public string BBBServerPrivateIp { get; }

        /// <summary>
        /// To determine which password to be used.
        /// </summary>
        /// <returns>Moderator password if <see cref="IsModerator"/> equals true, otherwise false.</returns>
#pragma warning disable CA1024 // Use properties where appropriate
        public string GetPassword()
#pragma warning restore CA1024 // Use properties where appropriate
        {
            return IsModerator ? ModeratorPassword : AttendeePassword;
        }
    }

    public class GetMeetingInfoUrlParams
    {
        public GetMeetingInfoUrlParams(
            Guid meetingId,
            string secretKey,
            string baseApiUrl,
            string bbbServerPrivateIp)
        {
            MeetingId = meetingId;
            SecretKey = secretKey;
            BaseApiUrl = baseApiUrl;
            BBBServerPrivateIp = bbbServerPrivateIp;
        }

        public Guid MeetingId { get; }

        public string SecretKey { get; }

        public string BaseApiUrl { get; }

        public string BBBServerPrivateIp { get; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
