using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.TaskController;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Sharing.Home;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CatalogueViewModel : BasePageViewModel
    {
        private readonly ICommonServices _commonService;
        private readonly ILearnerBackendService _learnerBackendService;

        private bool _isRefreshing;
        private int _newlyAddedTotalCount;
        private int _recommendationOrgTotalCount;
        private int _recommendationsTotalCount;
        private string _userId;

        private List<ItemCard> _mySuggestionCourseCardList;
        private List<ItemCard> _newlyAddedCourseCardList;
        private List<ItemCard> _recommendationByOrgCollectionCardList;

        private Dictionary<string, bool> _widgetConfigs = new Dictionary<string, bool>();

        public CatalogueViewModel()
        {
            CachingMode = PageCachingMode.None;

            _commonService = DependencyService.Resolve<ICommonServices>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
        }

        public SharingSectionViewModel SharingSection { get; set; } = new SharingSectionViewModel();

        public ICommand RefreshCommand => new Command(async () => await InitScreen());

        public LearnerObservableCollection<ItemCard> MySuggestionCollection { get; set; }

        public LearnerObservableCollection<ItemCard> NewlyAddedCollection { get; set; }

        public LearnerObservableCollection<ItemCard> RecommendationByOrgCollection { get; set; }

        public int NewlyAddedTotalCount
        {
            get
            {
                return _newlyAddedTotalCount;
            }

            set
            {
                _newlyAddedTotalCount = value;
                RaisePropertyChanged(() => NewlyAddedTotalCount);
            }
        }

        public int RecommendationOrgTotalCount
        {
            get
            {
                return _recommendationOrgTotalCount;
            }

            set
            {
                _recommendationOrgTotalCount = value;
                RaisePropertyChanged(() => RecommendationOrgTotalCount);
            }
        }

        public int RecommendationsTotalCount
        {
            get
            {
                return _recommendationsTotalCount;
            }

            set
            {
                _recommendationsTotalCount = value;
                RaisePropertyChanged(() => RecommendationsTotalCount);
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
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
                if (_isRefreshing == false)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public override string PageTitle { get; } = "Catalogue";

        public override string RoutingName => NavigationRoutes.Catalogue;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await LoadWidgetConfigs();
            await InitScreen();
        }

        private async Task InitScreen()
        {
            IsRefreshing = true;
            var taskRunner = new ParallelTaskRunner();

            // Note: We use list because we want to write code as a flow instead of passing params as params[].
            InitUserData();

            SharingSection.IsShowSharingSection = _widgetConfigs.ContainsKey(WidgetKeys.HomeSharedByOtherUsersShow) && _widgetConfigs[WidgetKeys.HomeSharedByOtherUsersShow];

            try
            {
                await taskRunner.RunAsync(
                    SharingSection.LoadData,
                    InitDataForMyRecommendationsCollection,
                    InitDataForMyRecommendationOrganizationCollection,
                    InitDataForNewlyAddedCollection);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadWidgetConfigs()
        {
            // Get widgets configs
            var widgetConfigsResult = await ExecuteBackendService(() => _learnerBackendService.GetUserPreference(new string[] { }));
            if (!widgetConfigsResult.HasEmptyResult() && !widgetConfigsResult.IsError)
            {
                _widgetConfigs = widgetConfigsResult.Payload.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        private void InitUserData()
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties != null)
            {
                _userId = accountProperties.User.Sub;
            }
        }

        private async Task InitDataForNewlyAddedCollection()
        {
            _newlyAddedCourseCardList = await _commonService.GetNewlyAddedCollection(totalCount: count =>
            {
                NewlyAddedTotalCount = count;
            });

            NewlyAddedCollection = new LearnerObservableCollection<ItemCard>(_newlyAddedCourseCardList);
            RaisePropertyChanged(() => NewlyAddedCollection);
        }

        private async Task InitDataForMyRecommendationsCollection()
        {
            _mySuggestionCourseCardList = await _commonService.GetRecommendationsCollection(_userId, totalCount: count =>
            {
                RecommendationsTotalCount = count;
            });

            MySuggestionCollection = new LearnerObservableCollection<ItemCard>(_mySuggestionCourseCardList);
            RaisePropertyChanged(() => MySuggestionCollection);
        }

        private async Task InitDataForMyRecommendationOrganizationCollection()
        {
            _recommendationByOrgCollectionCardList = await _commonService.GetRecommendationOrganizationCollection(totalCount: count =>
            {
                RecommendationOrgTotalCount = count;
            });
            RecommendationByOrgCollection = new LearnerObservableCollection<ItemCard>(_recommendationByOrgCollectionCardList);
            RaisePropertyChanged(() => RecommendationByOrgCollection);
        }
    }
}
