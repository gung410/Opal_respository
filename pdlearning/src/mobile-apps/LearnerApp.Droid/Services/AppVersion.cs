using Android.Content.PM;
using LearnerApp.Droid.Services;
using LearnerApp.PlatformServices;

[assembly: Xamarin.Forms.Dependency(typeof(AppVersion))]

namespace LearnerApp.Droid.Services
{
    public class AppVersion : IAppVersion
    {
        public string GetVersion()
        {
            return GetPackageInfo().VersionName;
        }

        public string GetBuildNumber()
        {
            return GetPackageInfo().LongVersionCode.ToString();
        }

        private PackageInfo GetPackageInfo()
        {
            var context = Android.App.Application.Context;
            PackageManager manager = context.PackageManager;
            PackageInfo info = manager?.GetPackageInfo(context.PackageName, 0);

            return info;
        }
    }
}
