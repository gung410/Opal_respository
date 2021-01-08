using Microsoft.AspNetCore.Http;

namespace Microservice.WebinarProxy.Application.Services
{
    public interface IChecksumHelper
    {
        /// <summary>
        /// Get checksum value from query list.
        /// </summary>
        /// <param name="queries">Query collection from request.</param>
        /// <returns>The checksum value or <see cref="string.Empty"/> if the checksum param does not exist.</returns>
        string GetChecksum(IQueryCollection queries);

        /// <summary>
        /// Validate checksum by compare the provided checksum with system checksum.
        /// </summary>
        /// <param name="queries">The collection of parameter and value.</param>
        /// <param name="clientChecksum">The client checksum.</param>
        /// <returns>True if the provided checksum equals the checksum was build from list parameters.</returns>
        bool ValidateClientChecksum(IQueryCollection queries, string clientChecksum);

        string BuildChecksumFromQuery(string query, string checksumSecretKey);
    }
}
