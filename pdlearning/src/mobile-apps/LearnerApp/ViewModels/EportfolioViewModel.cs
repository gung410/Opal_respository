using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class EportfolioViewModel : BasePageViewModel
    {
        public IdmResponse IdmResponse;
        public string ReturnUrl;

        private readonly IIdmBackendService _idmBackendService;
        private readonly IIdentityService _identityService;

        private string _eportfolioSourse;

        public EportfolioViewModel()
        {
            CachingMode = PageCachingMode.None;

            _identityService = DependencyService.Resolve<IIdentityService>();
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public override string PageTitle { get; } = "E-Portfolio";

        public override string RoutingName => NavigationRoutes.EPortfolio;

        public string EportfolioSourse
        {
            get
            {
                return _eportfolioSourse;
            }

            set
            {
                _eportfolioSourse = value;
                RaisePropertyChanged(() => EportfolioSourse);
            }
        }

        public async Task<string> GetAccessToken()
        {
            var properties = await _identityService.GetAccountPropertiesAsync();

            return properties != null ? properties.AccessToken : string.Empty;
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            string redirectUrl = GlobalSettings.WebViewUrlEportfolio;

            var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));

            if (apiResult.IsError || apiResult.HasEmptyResult())
            {
                return;
            }

            ReturnUrl = redirectUrl;

            IdmResponse = apiResult.Payload;

            EportfolioSourse = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";
        }
    }
}
