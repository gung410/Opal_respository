using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.Services.ServiceManager;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Sharing.Home
{
    public class SharingListPageViewModel : BasePageViewModel
    {
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler();
        private readonly ShareBackendServiceManager _shareBackendServiceManager;

        private int _totalCount;
        private bool _isLoading = true;
        private bool _ableToLoadMore = false;

        public SharingListPageViewModel()
        {
            _shareBackendServiceManager = new ShareBackendServiceManager();
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.SharingList;

        public LearnerObservableCollection<SharingContentItemViewModel> SharingSource { get; private set; }
            = new LearnerObservableCollection<SharingContentItemViewModel>();

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
                RaisePropertyChanged(() => ShouldLoadMore);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsRefreshingForRefreshView);
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

        public bool ShouldLoadMore
        {
            get
            {
                return _ableToLoadMore && TotalCount > (SharingSource?.Count ?? 0);
            }
        }

        public ICommand LoadMoreCommand => new Command(async () =>
            await _loadMoreStressActionHandler.RunAsync(
                async () => await LoadData(true)));

        public ICommand RefreshCommand => new Command(async () =>
            await _loadMoreStressActionHandler.RunAsync(
                async () => await LoadData(false)));

        public async Task LoadData(bool loadMore)
        {
            if (!loadMore)
            {
                IsLoading = true;
                _ableToLoadMore = true;
                TotalCount = 0;
            }

            var sharings = await _shareBackendServiceManager.GetShares(SharingSource.Count);

            if (sharings == null)
            {
                IsLoading = false;
                _ableToLoadMore = false;
                RaisePropertyChanged(() => ShouldLoadMore);
                return;
            }

            var sharingSource =
                sharings.Items.Select(x => new SharingContentItemViewModel(x)).ToList();

            TotalCount = sharings.TotalCount;
            if (!loadMore)
            {
                SharingSource = new LearnerObservableCollection<SharingContentItemViewModel>(sharingSource);
                RaisePropertyChanged(() => SharingSource);
                IsLoading = false;
            }
            else
            {
                SharingSource.AddRange(sharingSource);
            }

            if (SharingSource.Count >= TotalCount)
            {
                _ableToLoadMore = false;
                RaisePropertyChanged(() => ShouldLoadMore);
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            await LoadData(false);
        }
    }
}
