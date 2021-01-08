using System.Threading.Tasks;
using LearnerApp.Models.Learner;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IWebinarBackendService
    {
        [Get("/api/webinar/sessions/{source}/{sessionId}/joinUrl")]
        public Task<JoinWebinarUrlResponse> GetWebinarJoinUrl(string source, string sessionId);
    }
}
