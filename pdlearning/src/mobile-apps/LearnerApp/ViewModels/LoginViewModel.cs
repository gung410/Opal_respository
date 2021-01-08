using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.PlatformServices;
using LearnerApp.Services.Backend;
using LearnerApp.Services.DataManager.InitLogin;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class LoginViewModel : BasePageViewModel, IRoutingAware
    {
        private readonly IIdentityService _identityService;
        private readonly IOAuth2PkceSupport _pkceSupport;
        private readonly IPortalBackendService _portalBackendService;
        private readonly ICloudFrontBackendService _cloudFrontBackendService;
        private readonly IUserManagementBackendService _onBoardingBackendService;
        private readonly IMetadataBackendService _metadataBackendService;

        private bool _isVisibleLogin = true;

        public LoginViewModel()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
            _pkceSupport = DependencyService.Resolve<IOAuth2PkceSupport>();
            _metadataBackendService = CreateRestClientFor<IMetadataBackendService>(GlobalSettings.BackendServiceTagging);
            _portalBackendService = CreateRestClientFor<IPortalBackendService>(GlobalSettings.BackendServicePortal);
            _cloudFrontBackendService = CreateRestClientFor<ICloudFrontBackendService>(GlobalSettings.BackendServiceCloudFront);
            _onBoardingBackendService = CreateRestClientFor<IUserManagementBackendService>(GlobalSettings.BackendServiceUserManagement);
        }

        public bool IsVisibleLogin
        {
            get
            {
                return _isVisibleLogin;
            }

            set
            {
                _isVisibleLogin = value;
                RaisePropertyChanged(() => IsVisibleLogin);
            }
        }

        public ICommand LoginCommand => new Command(async () => await Login());

        public ICommand TermsAndConditionCommand => new Command(async () => await OpenTermsAndConditionBrowserAsync());

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Login;

        public async Task Login()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                // Set IsRedirectLoginPage = false to reset state of message to display error message correctly.
                ErrorState.IsRedirectLoginPage = false;

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await DialogService.ShowAlertAsync("No internet connection", cancelTextBtn: "Try again");
                }
                else
                {
                    var identityModel = await _identityService.Authenticate(_pkceSupport);

                    using (this.DialogService.DisplayLoadingIndicator())
                    {
                        if (!identityModel.IsAuthenticated)
                        {
                            IsBusy = false;
                            return;
                        }

                        IsVisibleLogin = false;

                        var hasPermission = await CheckPermissionAsync();
                        if (hasPermission)
                        {
                            var onBoardingState = await CheckFinishOnBoarding(identityModel.User.Sub);
                            identityModel.SetOnBoardedStatus(onBoardingState);

                            // Re-store account properties check onboarding
                            await _identityService.SetAccountPropertiesAsync(JsonConvert.SerializeObject(identityModel));

                            // Get and store cloudfront cookie to secure storage.
                            var cookieInfo = await _cloudFrontBackendService.GetCloudFrontCookieInfo();
                            await _identityService.StoreCloudFrontCookieInfo(cookieInfo);
                            await _identityService.SetupCloudFrontCookieForImageService();

                            // Get initial meta-data tagging
                            var initLoginDataManager = new InitLoginDataManager();
                            await initLoginDataManager.InitLoginData();

                            if (OnBoardingState.OnBoarded.Equals(onBoardingState))
                            {
                                await this.NavigationService.NavigateToMainTabItem(NavigationRoutes.Home, null);
                            }
                            else
                            {
                                await this.NavigationService.NavigateToAsync<OnBoardingViewModel>();
                            }
                        }
                        else
                        {
                            IsVisibleLogin = true;
                            await this.NavigationService.NavigateToAsync<LoginViewModel>();
                        }
                    }
                }

                IsBusy = false;
            }
        }

        private async Task OpenTermsAndConditionBrowserAsync()
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("Url", GlobalSettings.WebViewUrlTermOfUse);
            await this.NavigationService.NavigateToAsync<TermsOfUseViewModel>(parameters);
        }

        /// <summary>
        /// Check whether a user has finished the on boarding process or not.
        /// </summary>
        /// <param name="userId">The logged user id.</param>
        /// <returns>"true" if user has finished the on boarding process, otherwise "false".
        /// The reason we return string value (instead of boolean) because we want to add this result to AccountProperties, which is containing string values only.
        /// </returns>
        private async Task<string> CheckFinishOnBoarding(string userId)
        {
            var userResponse = await ExecuteBackendService(() => _onBoardingBackendService.GetUserManagement(userId));
            if (userResponse.HasEmptyResult())
            {
                return OnBoardingState.NotYetOnBoarded;
            }

            var onBoarding = userResponse.Payload;

            if (onBoarding != null
                && onBoarding.Items.Count >= 1
                && onBoarding.Items[0].JsonDynamicAttributes != null
                && onBoarding.Items[0].JsonDynamicAttributes.FinishOnBoarding)
            {
                return OnBoardingState.OnBoarded;
            }

            return OnBoardingState.NotYetOnBoarded;
        }

        private async Task<bool> CheckPermissionAsync()
        {
            try
            {
                await _portalBackendService.GetAllowedSites();
            }
            catch (Exception exception)
            {
                ExceptionHandler.HandleException(exception);

                _identityService.RemoveAccountProperties();

                _identityService.RemoveCloudFrontCookieInfo();

                _pkceSupport.ManualClearAllCookies();

                await this.DialogService.ShowAlertAsync("Can't check permission for user.");

                return false;
            }

            return true;
        }
    }
}
