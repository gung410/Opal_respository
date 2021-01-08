using System.Threading.Tasks;
using LearnerApp.Models;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ICloudFrontBackendService
    {
        [Get("/api/cloudfront/getCookieInfo")]
        Task<CloudFrontCookieInfo> GetCloudFrontCookieInfo();
    }
}
