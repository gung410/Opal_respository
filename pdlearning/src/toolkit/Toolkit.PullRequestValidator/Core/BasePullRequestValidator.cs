using System.IO;
using System.Threading.Tasks;
using Conexus.Toolkit.PullRequestValidator.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Conexus.Toolkit.PullRequestValidator.Core
{
    public abstract class BasePullRequestValidator
    {
        protected static async Task<PullRequestInfo> ParseRequestBody(HttpRequest httpRequest)
        {
            using (var streamReader = new StreamReader(httpRequest.Body))
            {
                string requestBody = await streamReader.ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<PullRequestInfo>(requestBody);
                return data;
            }
        }
    }
}
