using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
{
    public interface IInternalHttpClientRequestService
    {
        public Task<T> GetAsync<T>(string token, string baseUrl, params (string, List<string>)[] payloads);
    }
}
