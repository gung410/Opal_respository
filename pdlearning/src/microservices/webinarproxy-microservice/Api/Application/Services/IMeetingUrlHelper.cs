using Microsoft.AspNetCore.Http;

namespace Microservice.WebinarProxy.Application.Services
{
    public interface IMeetingUrlHelper
    {
        /// <summary>
        /// Remove parameters that not is the join meeting parameter.
        /// </summary>
        /// <param name="queries">List request queries.</param>
        /// <returns>A query string with necessary parameters to build checksum. Eg: joinfullName=A&meetingID=1&password=pw&userID=id.</returns>
        string GetJoinMeetingQueryString(IQueryCollection queries);

        /// <summary>
        /// Append error code into the provided Url.
        /// </summary>
        /// <param name="baseUrl">The url that error code will be appended.</param>
        /// <param name="errorCode">The error code.</param>
        /// <returns>Full url was combined with provided error code.</returns>
#pragma warning disable CA1055 // URI-like return values should not be strings
        string BuildErrorUrl(string baseUrl, string errorCode);
#pragma warning restore CA1055 // URI-like return values should not be strings
    }
}
