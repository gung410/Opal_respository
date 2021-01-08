using System.Threading.Tasks;
using IdentityModel.Client;

namespace Conexus.Opal.Microservice.Infrastructure
{
    public interface IAuthenticationTokenService
    {
        Task<TokenResponse> GetToken();
    }
}
