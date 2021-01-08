using Xamarin.UITest;

namespace LearnerApp.UITest
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.InstalledApp("com.moe.opal.v2").StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}
