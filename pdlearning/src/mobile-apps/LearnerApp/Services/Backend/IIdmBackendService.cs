using System.Threading.Tasks;
using LearnerApp.Models;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IIdmBackendService
    {
        [Post("/api/tokens/passwordlessurl?returnUrl={loginPasswordLessUrl}")]
        Task<IdmResponse> GetLoginWithTokenUrl(string loginPasswordLessUrl);

        [Get("/connect/endsession?id_token_hint={idTokenHint}&post_logout_redirect_uri={redirectUrl}")]
        Task EndSession(string idTokenHint, string redirectUrl);
    }
}
