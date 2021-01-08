using System;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Services;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Download;
using LearnerApp.Services.ExceptionHandler;
using LearnerApp.Services.Identity;
using LearnerApp.Services.MediaPicker;
using LearnerApp.Services.Metadatas;
using LearnerApp.Services.Navigation;
using LearnerApp.Services.OpenUrl;
using LearnerApp.Views.Achievement;
using LearnerApp.Views.Achievement.Badge;
using LearnerApp.Views.Achievement.ECertificate;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AndroidSpecific = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LearnerApp
{
    public partial class App : Application
    {
        private IIdentityService _identityService;
        private ICommonServices _commonService;

        public App()
        {
            InitializeComponent();
            InitApp();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Init MediaElement
            Xamarin.Forms.Device.SetFlags(new string[] { "MediaElement_Experimental" });

            Application.Current.MainPage = new AppShell();

            await RegisterAppCenter();
        }

        protected override void OnSleep()
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAllAsync();
            }
        }

        protected override async void OnResume()
        {
            await TrackingStartApp();
        }

        private async Task TrackingStartApp()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();

            var isAuthenticated = await _identityService.IsAuthenticated();

            if (isAuthenticated)
            {
                _commonService = DependencyService.Resolve<ICommonServices>();
                await _commonService.LearningTrackingStartApp();
            }
        }

        private void InitApp()
        {
            RegisterSingletonDependencies();
            AndroidSpecific.Application.SetWindowSoftInputModeAdjust(this, AndroidSpecific.WindowSoftInputModeAdjust.Resize);
            IdentityService.SessionId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Register all services as singletons into Dependency Service that being used across the app.
        /// </summary>
        private void RegisterSingletonDependencies()
        {
            DependencyService.Register<IdentityService>();
            DependencyService.Register<NavigationService>();
            DependencyService.Register<DialogService>();
            DependencyService.Register<CommonServices>();
            DependencyService.Register<MetadataService>();
            DependencyService.Register<DialogService>();
            DependencyService.Register<OpenUrlService>();
            DependencyService.Register<FilePickerService>();
            DependencyService.Register<Downloader>();
            DependencyService.Register<ExceptionHandler>();
            DependencyService.Register<S3UploadService>();
        }

        private Task RegisterAppCenter()
        {
            AppCenter.Start(
                $"ios={GlobalSettings.AppCenterSecretIOS};android={GlobalSettings.AppCenterSecretAndroid}",
                typeof(Analytics),
                typeof(Crashes));

            return Task.CompletedTask;
        }
    }
}
