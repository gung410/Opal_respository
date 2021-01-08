using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class CommunityDetailViewModel : BasePageViewModel, INavigationAware
    {
        public IdmResponse IdmResponse;

        private readonly IIdmBackendService _idmBackendService;
        private string _returnUrl;
        private string _sourceUrl;

        public CommunityDetailViewModel()
        {
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
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

        public override string RoutingName => NavigationRoutes.CommunityDetails;

        protected async override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                string redirectUrl = $"{GlobalSettings.WebViewUrlSocial}/dashboard?isMobile=true";

                var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
                if (apiResult.IsError || apiResult.HasEmptyResult())
                {
                    return;
                }

                ReturnUrl = navigationParameters.GetParameter<string>("details-url");
                IdmResponse = apiResult.Payload;
                SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";
            }
        }
    }
}
