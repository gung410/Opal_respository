using System.Collections.Generic;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microservice.WebinarProxy.Application.Services
{
    public class MeetingUrlHelper : IMeetingUrlHelper
    {
        /// <inheritdoc />
        public string GetJoinMeetingQueryString(IQueryCollection queries)
        {
            var meetingParams = new List<string>
            {
                "area",
                "fullName",
                "meetingID",
                "password",
                "createTime",
                "userID",
                "webVoiceConf",
                "configToken",
                "defaultLayout",
                "avatarURL",
                "redirect",
                "clientURL",
                "joinViaHtml5",
            };

            var queryStringBuilder = new StringBuilder();

            int index = 0;
            foreach (KeyValuePair<string, StringValues> query in queries)
            {
                if (meetingParams.Contains(query.Key))
                {
                    var separateChar = index == 0 ? string.Empty : "&";
                    queryStringBuilder.Append($"{separateChar}{query.Key}={HttpUtility.UrlEncode(query.Value)}");
                }

                index += 1;
            }

            return queryStringBuilder.ToString();
        }

        /// <inheritdoc />
#pragma warning disable CA1055 // URI-like return values should not be strings
        public string BuildErrorUrl(string baseUrl, string errorCode)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(errorCode))
            {
                return baseUrl;
            }

            return $"{baseUrl}/{errorCode}";
        }
    }
}
