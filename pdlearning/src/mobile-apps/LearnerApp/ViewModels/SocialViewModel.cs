using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class SocialViewModel : BasePageViewModel
    {
        public IdmResponse IdmResponse;
        public string ReturnUrl;

        private readonly IIdmBackendService _idmBackendService;
        private string _sourceUrl;

        public SocialViewModel()
        {
            CachingMode = PageCachingMode.None;

            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        public override string PageTitle { get; } = "Social";

        public override string RoutingName => NavigationRoutes.Social;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            string redirectUrl = $"{GlobalSettings.WebViewUrlSocial}/dashboard?isMobile=true";

            var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
            if (apiResult.IsError || apiResult.HasEmptyResult())
            {
                return;
            }

            ReturnUrl = redirectUrl;
            IdmResponse = apiResult.Payload;
            SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";
        }
    }
}
