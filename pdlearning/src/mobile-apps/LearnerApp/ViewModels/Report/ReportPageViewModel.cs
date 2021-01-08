using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels.Report
{
    public class ReportPageViewModel : BasePageViewModel
    {
        public IdmResponse IdmResponse;
        public string ReturnUrl;

        private IIdmBackendService _idmBackendService;
        private string _sourceUrl;

        public ReportPageViewModel()
        {
            CachingMode = PageCachingMode.None;

            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Report;

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            string redirectUrl = GlobalSettings.WebViewUrlReport;

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
