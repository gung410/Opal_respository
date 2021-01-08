using System.Threading.Tasks;
using FFImageLoading;
using LearnerApp.Models;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class EditProfileViewModel : BasePageViewModel, INavigationAware
    {
        public IdmResponse IdmResponse;

        private string _returnUrl;

        private string _sourceUrl;

        public EditProfileViewModel()
        {
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                if (!string.IsNullOrEmpty(value))
                {
                    RaisePropertyChanged(() => SourceUrl);
                }
            }
        }

        public string ReturnUrl
        {
            get => _returnUrl;
            set
            {
                _returnUrl = value;
                if (!string.IsNullOrEmpty(value))
                {
                    RaisePropertyChanged(() => ReturnUrl);
                }
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.EditProfile;

        public async Task RedirectNavigationPage(bool isCancelClick)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                // We clean cache image because we use Cloudfont to display image but cloudfont same URL
                // But with an URL string, the actual property value won't change (it will always be the same URL), so the PropertyChanged is simply ignored
                // When set value of FFImageLoading control, we invalidate cache to ensure new value always updated, profile can be change profile picture in webview, web browser.
                await ImageService.Instance.InvalidateCacheAsync(FFImageLoading.Cache.CacheType.All);

                if (!isCancelClick)
                {
                    // We must delay 3 second to Image reload with same uri and new value
                    await Task.Delay(3000);
                }

                await NavigationService.NavigateToAsync(NavigationRoutes.Main, null);
            });
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            var idmResponse = navigationParameters.GetParameter<IdmResponse>("IdmResponse");

            ReturnUrl = navigationParameters.GetParameter<string>("ReturnUrl");

            IdmResponse = idmResponse;

            SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";

            return Task.CompletedTask;
        }
    }
}
