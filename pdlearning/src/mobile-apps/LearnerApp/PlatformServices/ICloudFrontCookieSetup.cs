using System.Net;
using System.Threading.Tasks;

namespace LearnerApp.PlatformServices
{
    public interface ICloudFrontCookieSetup
    {
        Task SetupCloudFrontCookie(CookieCollection cookies);
    }
}
