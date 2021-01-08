using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels.Home.OutstandingTasks
{
    public class OutstandingTaskListPageViewModel : BasePageViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly OutstandingTaskListHandler _outstandingListHandler;

        private int _totalCount;
        private bool _isLoading = true;
        private bool _ableToLoadMore = false;

        public OutstandingTaskListPageViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _outstandingListHandler = new OutstandingTaskListHandler(_learnerBackendService);
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.OutstandingTasks;

        public LearnerObservableCollection<object> OutstandingTasks { get; private set; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
                RaisePropertyChanged(() => RemainingItemThreshold);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
                RaisePropertyChanged(() => OutstandingTasks);
            }
        }

        public int RemainingItemThreshold
        {
            get
            {
                return _ableToLoadMore && TotalCount > (OutstandingTasks?.Count ?? 0) ? 1 : -1;
            }
        }

        public async Task RefreshData()
        {
            IsLoading = true;
            _ableToLoadMore = true;
            TotalCount = 0;
            var outstandingTasks =
                await ExecuteBackendService(() => _learnerBackendService.GetOutstandingTasks());

            if (outstandingTasks.IsError)
            {
                IsLoading = false;
                return;
            }

            var outstandingTasksViewModels =
                outstandingTasks.Payload.Items.Select(x => new OutstandingTaskItemViewModel(x)).ToList();

            OutstandingTasks = new LearnerObservableCollection<object>(outstandingTasksViewModels);
            TotalCount = outstandingTasks.Payload.TotalCount;
            IsLoading = false;
        }

        public async Task LoadMore()
        {
            var outstandingTasks =
                await ExecuteBackendService(() => _learnerBackendService.GetOutstandingTasks(OutstandingTasks.Count));

            if (outstandingTasks.IsError)
            {
                IsLoading = false;
                _ableToLoadMore = false;
                return;
            }

            var outstandingTasksViewModels =
                outstandingTasks.Payload.Items.Select(x => new OutstandingTaskItemViewModel(x)).ToList();

            OutstandingTasks.AddRange(outstandingTasksViewModels);
            TotalCount = outstandingTasks.Payload.TotalCount;
            RaisePropertyChanged(() => RemainingItemThreshold);
        }

        public override async Task OnComeBack()
        {
            await _outstandingListHandler.ReloadRowIfNeeded(
                OutstandingTasks,
                TotalCount,
                (totalCount) => TotalCount = totalCount);
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            await RefreshData();
        }
    }
}
