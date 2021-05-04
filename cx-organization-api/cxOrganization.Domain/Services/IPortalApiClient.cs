using cxPlatform.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Services
{
    /// <summary>
    /// The portal api client which mastering the permissions of the user.
    /// </summary>
    public interface IPortalApiClient
    {
        /// <summary>
        /// Gets permission keys who is requesting the api.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IList<string>> GetPermissionKeys(string token, string cxToken);
    }
}
