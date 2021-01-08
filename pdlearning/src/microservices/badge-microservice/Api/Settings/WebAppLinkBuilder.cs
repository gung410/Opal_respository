using Microsoft.Extensions.Options;

namespace Microservice.Badge.Settings
{
    public class WebAppLinkBuilder
    {
        public static readonly string LearnerModule = "learner";

        private readonly OpalClientUrlOption _opalClientUrlOption;

        public WebAppLinkBuilder(IOptions<OpalClientUrlOption> opalClientUrlOption)
        {
            _opalClientUrlOption = opalClientUrlOption.Value;
        }

        // Learner Deeplink
#pragma warning disable CA1024 // Use properties where appropriate
        public string GetMyAchievementsLearnerLinkForCAMModule()
        {
            return $"{_opalClientUrlOption.OpalClientUrl}{LearnerModule}/detail/myachievements";
        }
#pragma warning restore CA1024 // Use properties where appropriate
    }
}
