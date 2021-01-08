using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Home.OutstandingTasks
{
    public class OutstandingTasksSectionViewModel : BaseViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly INavigationService _navigationService;
        private readonly OutstandingTaskListHandler _outstandingListHandler;

        private int _totalCount;
        private bool _isLoading = true;
        private bool _isShowList;
        private bool _isShowOutstandingSection;
        private HomeViewModel _homeViewModel;

        public OutstandingTasksSectionViewModel(HomeViewModel parent)
        {
            _homeViewModel = parent;
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _navigationService = DependencyService.Resolve<INavigationService>();
            _outstandingListHandler = new OutstandingTaskListHandler(_learnerBackendService);
        }

        public LearnerObservableCollection<object> OutstandingTasks { get; private set; }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => OutstandingTaskSectionTitle);
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
                RaisePropertyChanged(() => IsShowList);
            }
        }

        public bool IsShowList
        {
            get => _isLoading || TotalCount > 0;
            set
            {
                _isShowList = value;
                RaisePropertyChanged(() => IsShowList);
            }
        }

        public bool IsShowOutstandingSection
        {
            get
            {
                return _isShowOutstandingSection;
            }

            set
            {
                _isShowOutstandingSection = value;
                RaisePropertyChanged(() => IsShowOutstandingSection);
            }
        }

        public ICommand ShowAllOutstandingTasks => new Command(ShowAllOutStandingTasks);

        public string OutstandingTaskSectionTitle
        {
            get
            {
                string title = TextsResource.OUTSTANDING_TASKS;
                return title + (TotalCount > 0 ? $" ({TotalCount})" : string.Empty);
            }
        }

        public async Task LoadData()
        {
            if (!_homeViewModel.WidgetConfigs.ContainsKey(WidgetKeys.HomeOutstandingShow)
                || !_homeViewModel.WidgetConfigs[WidgetKeys.HomeOutstandingShow])
            {
                IsShowOutstandingSection = false;
                return;
            }
            else
            {
                IsShowOutstandingSection = true;
            }

            IsLoading = true;
            var outstandingTasks = await ExecuteBackendService(() => _learnerBackendService.GetOutstandingTasks(0, 3));

            if (outstandingTasks.IsError)
            {
                IsLoading = false;
                TotalCount = 0;
                return;
            }

            var outstandingTasksViewModels = outstandingTasks.Payload.Items.Take(3).Select(x => new OutstandingTaskItemViewModel(x)).ToList();

            TotalCount = outstandingTasks.Payload.TotalCount;
            OutstandingTasks = new LearnerObservableCollection<object>(outstandingTasksViewModels);
            RaisePropertyChanged(() => OutstandingTasks);
            IsLoading = false;
        }

        public async Task ReloadRowIfNeeded()
        {
            await _outstandingListHandler.ReloadRowIfNeeded(
                OutstandingTasks,
                TotalCount,
                (totalCount) => TotalCount = totalCount);
        }

        private void ShowAllOutStandingTasks(object obj)
        {
            _navigationService.NavigateToAsync<OutstandingTaskListPageViewModel>();
        }
    }
}
