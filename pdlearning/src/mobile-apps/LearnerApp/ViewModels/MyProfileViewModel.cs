using System;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.PlatformServices;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyProfileViewModel : BasePageViewModel, INavigationAware
    {
        private readonly IIdmBackendService _idmBackendService;
        private readonly IAppVersion _appVersion;

        private IdentityModel _accountProperties;
        private UserInfo _userInfo;
        private string _version;
        private string _copyrightText;
        private bool _isEditProfileBusy;
        private bool _isChangePasswordBusy;
        private string _avatar;

        public MyProfileViewModel()
        {
            _appVersion = DependencyService.Resolve<IAppVersion>();
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public ICommand OnClickEditProfileCommand => new Command(async () => await EditProfile());

        public ICommand OnClickChangePasswordCommand => new Command(async () => await ChangePassword());

        public ICommand OnClickReportVulnerabilityCommand => new Command(async () => await Browser.OpenAsync(new Uri("https://tech.gov.sg/report_vulnerability")));

        public ICommand OnClickTermOfUseCommand => new Command(async () => await TermOfUse());

        public ICommand OnClickPrivacyPolicyCommand => new Command(async () => await PrivacyPolicy());

        public string Version
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        public string CopyrightText
        {
            get
            {
                return _copyrightText;
            }

            set
            {
                _copyrightText = value;
                RaisePropertyChanged(() => CopyrightText);
            }
        }

        public UserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }

            set
            {
                _userInfo = value;
                RaisePropertyChanged(() => UserInfo);
            }
        }

        public string Avatar
        {
            get
            {
                return _avatar;
            }

            set
            {
                _avatar = value;
                RaisePropertyChanged(() => Avatar);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.MyProfile;

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            GetMyProfile();
            GetSiteInformation();
            return Task.CompletedTask;
        }

        private async Task ChangePassword()
        {
            if (!_isChangePasswordBusy)
            {
                _isChangePasswordBusy = true;

                var redirectUrl = $"{GlobalSettings.BackendServiceIdm}/Manage/ChangePassword?returnUrl={GlobalSettings.AuthRedirectUrl}://oauthchangepasswordredirect&isMobile=true";
                var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
                if (apiResult.IsError)
                {
                    return;
                }

                if (apiResult.HasEmptyResult())
                {
                    return;
                }

                var parameters = new NavigationParameters();
                parameters.SetParameter("IdmResponse", apiResult.Payload);
                parameters.SetParameter("ReturnUrl", redirectUrl);

                await this.NavigationService.NavigateToAsync<ChangePasswordViewModel>(parameters);

                _isChangePasswordBusy = false;
            }
        }

        private async Task EditProfile()
        {
            if (!_isEditProfileBusy)
            {
                _isEditProfileBusy = true;

                string redirectUrl = $"{GlobalSettings.WebViewUrlAccountProfile}?returnUrl={GlobalSettings.AuthRedirectUrl}&isMobile=true";
                var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
                if (apiResult.IsError)
                {
                    return;
                }

                if (apiResult.HasEmptyResult())
                {
                    return;
                }

                var parameters = new NavigationParameters();
                parameters.SetParameter("IdmResponse", apiResult.Payload);
                parameters.SetParameter("ReturnUrl", redirectUrl);

                await this.NavigationService.NavigateToAsync<EditProfileViewModel>(parameters);

                _isEditProfileBusy = false;
            }
        }

        private void GetMyProfile()
        {
            _accountProperties = Application.Current.Properties.GetAccountProperties();

            UserInfo = new UserInfo
            {
                Name = _accountProperties.User.Name,
                Email = _accountProperties.User.Email
            };

            Avatar = $"{GlobalSettings.BackendServiceUserAvatar}/{_accountProperties?.User?.Sub}";
        }

        private void GetSiteInformation()
        {
            Version = $"Version: {_appVersion.GetVersion()}. Build number: {_appVersion.GetBuildNumber()}";

            string lastUpdateString = string.IsNullOrEmpty(Application.Current.Properties.GetSiteInformation())
                ? GlobalSettings.LastUpdateString
                : Application.Current.Properties.GetSiteInformation();

            CopyrightText = $"Copyright © {DateTime.Now.Year} Ministry of Education, Singapore. All rights reserved. Last updated: {lastUpdateString}";
        }

        private async Task TermOfUse()
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("Url", GlobalSettings.WebViewUrlTermOfUse);
            await this.NavigationService.NavigateToAsync<TermsOfUseViewModel>(parameters);
        }

        private async Task PrivacyPolicy()
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("Url", GlobalSettings.WebViewUrlPrivacyPolicy);
            await this.NavigationService.NavigateToAsync<PrivacyPolicyViewModel>(parameters);
        }
    }
}
