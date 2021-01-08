using System.Linq;
using Microservice.WebinarProxy.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Helpers;

namespace Microservice.WebinarProxy.Application.Services
{
    public class ChecksumHelper : IChecksumHelper
    {
        private readonly BigBlueButtonOptions _bbbOption;
        private readonly IMeetingUrlHelper _urlHelper;

        public ChecksumHelper(
            IOptions<BigBlueButtonOptions> bbbConfiguration,
            IMeetingUrlHelper urlHelper)
        {
            _bbbOption = bbbConfiguration.Value;
            _urlHelper = urlHelper;
        }

        /// <inheritdoc />
        public string GetChecksum(IQueryCollection queries)
        {
            if (!queries.ContainsKey("checksum"))
            {
                return string.Empty;
            }

            return queries.FirstOrDefault(x => x.Key.Equals("checksum")).Value;
        }

        /// <inheritdoc />
        public bool ValidateClientChecksum(IQueryCollection queries, string clientChecksum)
        {
            var clientQueryString = _urlHelper.GetJoinMeetingQueryString(queries);
            var validChecksum = BuildChecksumFromQuery($"join{clientQueryString}", _bbbOption.ProxySecretKey);

            return validChecksum.Equals(clientChecksum);
        }

        public string BuildChecksumFromQuery(string query, string checksumSecretKey)
        {
            return EncryptionHelper.ComputeSha1Hash(query + checksumSecretKey);
        }
    }
}
