using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class OnBoardingViewModel : BasePageViewModel, INavigationAware, IRoutingAware
    {
        public IdmResponse IdmResponse;

        private readonly IIdmBackendService _idmBackendService;

        private readonly IUserManagementBackendService _onBoardingBackendService;

        private string _returnUrl;

        private string _sourceUrl;

        public OnBoardingViewModel()
        {
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
            _onBoardingBackendService = CreateRestClientFor<IUserManagementBackendService>(GlobalSettings.BackendServiceUserManagement);
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Onboarding;

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

        public async Task GetUserManagement()
        {
            var userInfo = await IdentityService.GetAccountPropertiesAsync();
            if (userInfo != null)
            {
                var userId = userInfo.User.Sub;
                var userResponse = await ExecuteBackendService(() => _onBoardingBackendService.GetUserManagement(userId));

                if (userResponse.HasEmptyResult())
                {
                    return;
                }

                var onBoarding = userResponse.Payload;

                if (onBoarding != null && onBoarding.Items.Count >= 1 && onBoarding.Items[0].JsonDynamicAttributes != null && onBoarding.Items[0].JsonDynamicAttributes.FinishOnBoarding)
                {
                    if (!string.IsNullOrEmpty(userInfo.OnBoarded))
                    {
                        userInfo.SetOnBoardedStatus("true");
                    }

                    await IdentityService.SetAccountPropertiesAsync(JsonConvert.SerializeObject(userInfo));
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await NavigationService.NavigateToMainTabItem(NavigationRoutes.Home, null);
                    });
                }
                else
                {
                    IdentityService.RemoveAccountProperties();
                    await this.DialogService.ShowAlertAsync("Update not successful");
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await NavigationService.NavigateToAsync<OnBoardingViewModel>();
                    });
                }
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            string returnUrl = $"{GlobalSettings.WebViewUrlOnBoarding}?returnUrl={GlobalSettings.AuthRedirectUrl}&ismobile=true";
            var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(returnUrl));
            if (apiResult.IsError)
            {
                return;
            }

            if (apiResult.HasEmptyResult())
            {
                return;
            }

            // IMPORTANT: There is an issue when receiving url result from STS that's having " character.
            // We need to ensure that there is no " character.
            ReturnUrl = returnUrl;

            IdmResponse = apiResult.Payload;

            SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";
        }
    }
}
