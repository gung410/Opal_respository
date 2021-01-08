using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Communication.Business.HttpClients
{
    public interface IGoogleAppInstanceServerHttpClient
    {
        Task<string[]> GetAppInstanceRelationsAsync(string instanceIdTokens);
        Task RemoveRelationshipMapAsync(ISet<string> instanceIdTokens, string topicName);
        Task AddRelationshipMapAsync(ISet<string> instanceIdTokens, string topicName);
        Task ValidateToken(string instanceIdToken);
    }
}
