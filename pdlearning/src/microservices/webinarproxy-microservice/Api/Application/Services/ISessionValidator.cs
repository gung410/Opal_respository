using System.Threading.Tasks;

namespace Microservice.WebinarProxy.Application.Services
{
    public interface ISessionValidator
    {
        public Task<bool> ValidateByToken(string token);
    }
}
