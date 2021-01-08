using Foundation;
using LearnerApp.iOS.Services;
using LearnerApp.PlatformServices;

[assembly: Xamarin.Forms.Dependency(typeof(AppVersion))]

namespace LearnerApp.iOS.Services
{
    public class AppVersion : IAppVersion
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }

        public string GetBuildNumber()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }
    }
}
