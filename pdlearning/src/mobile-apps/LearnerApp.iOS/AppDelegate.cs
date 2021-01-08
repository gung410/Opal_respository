using System;
using System.Threading.Tasks;
using FFImageLoading.Forms.Platform;
using Foundation;
using LearnerApp.Binding.Wogaa.iOS;
using LearnerApp.Services.ExceptionHandler;
using LibVLCSharp.Forms.Shared;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using Plugin.HybridWebView.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: ResolutionGroupName("LearnerApp")]

namespace LearnerApp.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private IExceptionHandler _exceptionHandler;

        private UIVisualEffectView _blurWindow = null;

        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            global::Xamarin.Forms.Forms.Init();

            HybridWebViewRenderer.Initialize();
            Rg.Plugins.Popup.Popup.Init();
            CachedImageRenderer.Init();
            CachedImageRenderer.InitImageSourceHandler();
            LibVLCSharpFormsRenderer.Init();
            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);

            LoadApplication(new App());

            RegisterGlobalException();
            if (NSBundle.MainBundle.BundleIdentifier == "com.opal2.moe.edu.sg")
            {
                Tracker.StartWith(Binding.Wogaa.iOS.Environment.Production);
            }
            else
            {
                Tracker.StartWith(Binding.Wogaa.iOS.Environment.Staging);
            }

#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Xamarin.Essentials.Platform.OpenUrl(app, url, options))
            {
                return true;
            }

            return base.OpenUrl(app, url, options);
        }

        public override void OnActivated(UIApplication application)
        {
            base.OnActivated(application);

            _blurWindow?.RemoveFromSuperview();
            _blurWindow?.Dispose();
            _blurWindow = null;
        }

        public override void OnResignActivation(UIApplication application)
        {
            base.OnResignActivation(application);

            using (var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark))
            {
                _blurWindow = new UIVisualEffectView(blurEffect)
                {
                    Frame = UIApplication.SharedApplication.KeyWindow.RootViewController.View.Bounds
                };
                UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(_blurWindow);
            }
        }

        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, IntPtr forWindow)
        {
            return DeviceOrientationImplementation.SupportedInterfaceOrientations;
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
    }
}
