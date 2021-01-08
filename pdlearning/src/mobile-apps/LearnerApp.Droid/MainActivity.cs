using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using FFImageLoading.Forms.Platform;
using LearnerApp.Common;
using LearnerApp.Services.ExceptionHandler;
using LibVLCSharp.Forms.Shared;
using Plugin.CurrentActivity;
using Plugin.DeviceOrientation;
using Plugin.HybridWebView.Droid;
using SG.Wogaa.Tracker;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace LearnerApp.Droid
{
    [Activity(Label = "OPAL2.0", Icon = "@mipmap/icon", Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity, IMainActivityWithStarting
    {
        private IExceptionHandler _exceptionHandler;
        private Action<int, Result, Intent> _resultCallback;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void StartActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback)
        {
            _resultCallback = resultCallback;

            if (requestCode == (int)ResultCodeIntent.FileChooserResultCode)
            {
                StartActivityForResult(intent, requestCode);
            }

            // We must check request code for take photo and cancel dialog because we use intent with null value can not override OnActivityResult
            else
            {
                OnActivityResultForIntent(requestCode, Result.Ok, intent);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            DeviceOrientationImplementation.NotifyOrientationChange(newConfig.Orientation);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            OnActivityResultForIntent(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            SupportActionBar.SetDisplayShowHomeEnabled(true); // Show or hide the default home button
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowCustomEnabled(true); // Enable overriding the default toolbar layout
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();
            global::Xamarin.Forms.Forms.Init(this, bundle);

            CheckAppPermissions();
            LibVLCSharpFormsRenderer.Init();
            HybridWebViewRenderer.Initialize();
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            CachedImageRenderer.Init(enableFastRenderer: false);
            CachedImageRenderer.InitImageViewHandler();
            CrossCurrentActivity.Current.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            LoadApplication(new App());

            RegisterGlobalException();

            if (ApplicationContext.PackageName == "com.opal2.moe.edu.sg")
            {
                Tracker.Start(this, SG.Wogaa.Tracker.Environment.Production);
            }
            else
            {
                Tracker.Start(this, SG.Wogaa.Tracker.Environment.Staging);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            Xamarin.Essentials.Platform.OnResume();

            Window.ClearFlags(WindowManagerFlags.Secure);
        }

        protected override void OnPause()
        {
            base.OnPause();

            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
        }

        private void RegisterGlobalException()
        {
            _exceptionHandler = DependencyService.Resolve<IExceptionHandler>();

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                _exceptionHandler.HandleException(args.ExceptionObject as Exception);
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                _exceptionHandler.HandleException(args.Exception);
            };
        }

        private void OnActivityResultForIntent(int requestCode, Result resultCode, Intent data)
        {
            if (_resultCallback != null)
            {
                _resultCallback(requestCode, resultCode, data);
                _resultCallback = null;
            }
        }

        private void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }

            if (PackageManager == null ||
                PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) ==
                Permission.Granted ||
                PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) ==
                Permission.Granted)
            {
                return;
            }

            var permissions = new[]
            {
                Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage
            };
            RequestPermissions(permissions, 1);
        }
    }
}
