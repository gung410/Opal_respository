using Android.Views;
using LearnerApp.Droid.Services;
using LearnerApp.PlatformServices.DroidServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(DroidSpecificServices))]

namespace LearnerApp.Droid.Services
{
    public class DroidSpecificServices : IDroidSpecificServices
    {
        private StatusBarVisibility? _originalSystemUiFlags;

        public void EnterFullscreenMode()
        {
            var mainActivity = MainApplication.CurrentContext as MainActivity;
            if (mainActivity == null)
            {
                return;
            }

            if (_originalSystemUiFlags == null)
            {
                _originalSystemUiFlags = mainActivity.Window.DecorView.SystemUiVisibility;
            }

            int uiOptions = (int)_originalSystemUiFlags;

            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            mainActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        public void ExitFullscreenMode()
        {
            var mainActivity = MainApplication.CurrentContext as MainActivity;
            if (mainActivity == null || _originalSystemUiFlags == null)
            {
                return;
            }

            mainActivity.Window.DecorView.SystemUiVisibility = _originalSystemUiFlags.Value;
        }
    }
}
