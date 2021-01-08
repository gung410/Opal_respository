using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetAllUserPreferencesQuery : BaseThunderQuery<List<UserPreferenceModel>>
    {
        public GetAllUserPreferencesQuery(List<string> keys)
        {
            Keys = keys;
        }

        public List<string> Keys { get; set; }
    }
}
