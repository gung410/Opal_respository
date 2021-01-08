using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.Navigation;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Sharing;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Achievement.ECertificate
{
    public class AchievementECertificatePageViewModel : BasePageViewModel
    {
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler();
        private readonly AchievementBackendServiceManager _achievementBackendServiceManager;

        private int _page = 1;
        private int _totalCount;
        private bool _isLoading = true;

        public AchievementECertificatePageViewModel()
        {
            _achievementBackendServiceManager = new AchievementBackendServiceManager();
        }

        public LearnerObservableCollection<AchievementECertificateItemViewModel> ECertificateSource { get; private set; }
            = new LearnerObservableCollection<AchievementECertificateItemViewModel>();

        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                RaisePropertyChanged(() => Page);
            }
        }

        public int TotalPageNumber
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / GlobalSettings.MaxResultPerPage);
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
                RaisePropertyChanged(() => TotalPageNumber);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

        public bool IsRefreshingForRefreshView
        {
            get
            {
                return false;
            }

            set
            {
                if (_isLoading == false)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.AchievementECertificate;

        public ICommand LoadPageCommand => new Command(async () =>
        {
            await LoadData();
        });

        public ICommand RefreshCommand => new Command(async () =>
        {
            Page = 1;
            await _loadMoreStressActionHandler.RunAsync(
                async () => await LoadData());
        });

        public async Task LoadData()
        {
            IsLoading = true;
            var eCerts = await _achievementBackendServiceManager.GetAchievementCertificate(
                (Page - 1) * GlobalSettings.MaxResultPerPage);

            if (eCerts == null)
            {
                IsLoading = false;
                return;
            }

            var eCertsVm =
                eCerts.Items.Select(x => new AchievementECertificateItemViewModel(x)).ToList();

            TotalCount = eCerts.TotalCount;
            ECertificateSource = new LearnerObservableCollection<AchievementECertificateItemViewModel>(eCertsVm);
            RaisePropertyChanged(() => ECertificateSource);
            IsLoading = false;
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            await LoadData();
        }
    }
}
