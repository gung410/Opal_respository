using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public interface IIdentityServerClientService
    {
        Task<string> GetAccessToken();

        Task<UserIdmResponseInfo> GetUsers(string orgUnit = "", string role = "", int pageIndex = 1, int pageSize = 100);
    }
}