using System.Net.Http;
using System.Threading.Tasks;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface IOpalIdpBackendService
    {
        [Post("/opal-account/api/data/profile/upload")]
        Task UploadProfile([Body] StreamContent file);
    }
}
