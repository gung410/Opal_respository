using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public enum LearningPathSearchFilter
    {
        Owner,
        Shared,
        Recommendations
    }

    public class MyLearningPathsViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommonServices _commonService;

        private List<LearningPath> _myOwnLearningPathsCollection;
        private List<LearningPath> _sharedLearningPathsCollection;
        private List<LearningPath> _recommendationLearningPathsCollection;

        private SearchGroupFilterData _filterData;
        private List<string> _searchTags;
        private string _currentSearchFilterGroup;
        private string _searchLearningPathKeyword;
        private bool _isRefreshing;
        private bool _isVisibleLearningSearch;
        private int _searchFilterTotalCount;
        private int _learningPathFromLMMPaging = 1;
        private int _ownLearningPathsTotalCount;
        private int _sharedToMeLearningPathsTotalCount;
        private int _recommendationLearningPathTotalCount;

        // Permission
        private bool _isVisibileLearningPathCUD = true;

        public MyLearningPathsViewModel()
        {
            _navigationService = DependencyService.Resolve<INavigationService>();
            _commonService = DependencyService.Resolve<ICommonServices>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);

            _searchTags = new List<string>
            {
                "contains"
            };

            var metadataTagging = App.Current.Properties.GetMetadataTagging();
            if (!metadataTagging.IsNullOrEmpty())
            {
                var mainSubjectArea = metadataTagging.AsParallel().Where(p => p.GroupCode == MetadataTagGroupCourse.MAINSUBJECTAREA).Select(p => p.TagId.ToString()).ToList();
                if (!mainSubjectArea.IsNullOrEmpty())
                {
                    _searchTags.AddRange(mainSubjectArea);
                }
            }

            MessagingCenter.Subscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths", async (sender, arg) =>
            {
                if (IsVisibleLearningSearch)
                {
                    await OnSearchLearningPath();
                }
                else
                {
                    OnRefreshing(null);
                }
            });

            MessagingCenter.Subscribe<MyLearningPathsCreateNewViewModel>(this, "deleted-learning-path", async (sender) =>
            {
                if (IsVisibleLearningSearch)
                {
                    await OnSearchLearningPath();
                }
                else
                {
                    OnRefreshing(null);
                }
            });
        }

        public ICommand CreateNewLearningPathCommand => new Command(CreateNewLearningPaths);

        public ICommand RefreshingCommand => new Command(OnRefreshing);

        public ICommand SearchLearningPathCommand => new Command(async () => await OnSearchLearningPath());

        public ICommand SearchCourseFilterCommand => new Command(async (currentFilter) => await OnSearchLearningPathFilter(currentFilter as Dictionary<string, int>));

        public ICommand BackSearchCourseCommand => new Command(OnDismissSearch);

        public ICommand SearchRefreshCommand => new Command(async () => await OnSearchLearningPathRefresh());

        public ICommand SearchLoadmoreCommand => new Command(async () => await OnLoadmoreSearchLearningPath());

        public LearnerObservableCollection<LearningPath> MyOwnLearningPathsCollection { get; set; }

        public LearnerObservableCollection<LearningPath> RecommendationLearningPathsCollection { get; set; }

        public LearnerObservableCollection<LearningPath> SharedLearningPathsCollection { get; set; }

        public LearnerObservableCollection<LearningPath> SearchCollection { get; set; }

        public SearchGroupFilterData FilterData
        {
            get
            {
                return _filterData;
            }

            set
            {
                _filterData = value;
                RaisePropertyChanged(() => FilterData);
            }
        }

        public int RecommendationLearningPathTotalCount
        {
            get
            {
                return _recommendationLearningPathTotalCount;
            }

            set
            {
                _recommendationLearningPathTotalCount = value;
                RaisePropertyChanged(() => RecommendationLearningPathTotalCount);
            }
        }

        public int SharedToMeLearningPathsTotalCount
        {
            get
            {
                return _sharedToMeLearningPathsTotalCount;
            }

            set
            {
                _sharedToMeLearningPathsTotalCount = value;
                RaisePropertyChanged(() => SharedToMeLearningPathsTotalCount);
            }
        }

        public int OwnLearningPathsTotalCount
        {
            get
            {
                return _ownLearningPathsTotalCount;
            }

            set
            {
                _ownLearningPathsTotalCount = value;
                RaisePropertyChanged(() => OwnLearningPathsTotalCount);
            }
        }

        public int SearchFilterTotalCount
        {
            get
            {
                return _searchFilterTotalCount;
            }

            set
            {
                _searchFilterTotalCount = value;
                RaisePropertyChanged(() => SearchFilterTotalCount);
            }
        }

        public string SearchLearningPathKeyword
        {
            get
            {
                return _searchLearningPathKeyword;
            }

            set
            {
                _searchLearningPathKeyword = value;

                if (string.IsNullOrEmpty(_searchLearningPathKeyword))
                {
                    IsVisibleLearningSearch = false;
                }

                RaisePropertyChanged(() => SearchLearningPathKeyword);
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

        public bool IsVisibleLearningSearch
        {
            get
            {
                return _isVisibleLearningSearch;
            }

            set
            {
                _isVisibleLearningSearch = value;
                RaisePropertyChanged(() => IsVisibleLearningSearch);
            }
        }

        public bool IsVisibileLearningPathCUD
        {
            get
            {
                return _isVisibileLearningPathCUD;
            }

            set
            {
                _isVisibileLearningPathCUD = value;
                RaisePropertyChanged(() => IsVisibileLearningPathCUD);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel, LearningPath>(this, "update-learning-paths");
            MessagingCenter.Unsubscribe<MyLearningPathsCreateNewViewModel>(this, "deleted-learning-path");
        }

        public Task InitLearningPaths()
        {
            IsVisibileLearningPathCUD = PermissionHelper.GetPermissionForLearningPathCUD();

            using (DialogService.DisplayLoadingIndicator())
            {
                MyOwnLearningPathsCollection = new LearnerObservableCollection<LearningPath>();
                SharedLearningPathsCollection = new LearnerObservableCollection<LearningPath>();
                RecommendationLearningPathsCollection = new LearnerObservableCollection<LearningPath>();

                var initTasks = new List<Func<Task>>
                {
                    LoadMyOwnLearningPaths,
                    LoadSharedToMeLearningPaths,
                    LoadRecommendationLearningPath
                }.GetEnumerator();
                return TaskHelper.RunSequential(
                onComplete: () =>
                {
                    MyOwnLearningPathsCollection.AddRange(_myOwnLearningPathsCollection);
                    SharedLearningPathsCollection.AddRange(_sharedLearningPathsCollection);
                    RecommendationLearningPathsCollection.AddRange(_recommendationLearningPathsCollection);

                    RaisePropertyChanged(() => MyOwnLearningPathsCollection);
                    RaisePropertyChanged(() => SharedLearningPathsCollection);
                    RaisePropertyChanged(() => RecommendationLearningPathsCollection);

                    _myOwnLearningPathsCollection = null;
                    _sharedLearningPathsCollection = null;
                    _recommendationLearningPathsCollection = null;
                },
                actions: initTasks);
            }
        }

        private async Task LoadMyOwnLearningPaths()
        {
            var listLearningPath = await ExecuteBackendService(() => _learnerBackendService.SearchLearningPath(new
            {
                SkipCount = 0,
                MaxResultCount = GlobalSettings.MaxResultPerPage
            }));

            if (!listLearningPath.HasEmptyResult() && listLearningPath.Payload.TotalCount > 0)
            {
                var sortedItems = listLearningPath.Payload.Items.OrderByDescending(p => p.CreatedDate).ToList();

                _myOwnLearningPathsCollection = sortedItems;
            }

            OwnLearningPathsTotalCount = listLearningPath.Payload.TotalCount;
        }

        private async Task LoadSharedToMeLearningPaths()
        {
            _sharedLearningPathsCollection = new List<LearningPath>();

            var listSharedLearningPath = await ExecuteBackendService(() => _learnerBackendService.GetLearningPathsSharedToMe(skipCount: 0));

            if (!listSharedLearningPath.HasEmptyResult() && listSharedLearningPath.Payload.TotalCount > 0)
            {
                var sortedItems = listSharedLearningPath.Payload.Items.OrderByDescending(p => p.CreatedDate).ToList();

                _sharedLearningPathsCollection = sortedItems;
            }

            SharedToMeLearningPathsTotalCount = listSharedLearningPath.Payload.TotalCount;
        }

        private async Task LoadRecommendationLearningPath()
        {
            _recommendationLearningPathsCollection = new List<LearningPath>();

            var learningPaths = await _commonService.RecommendationLearningPathsCollection(
                userId: Application.Current.Properties.GetAccountProperties().User.Sub,
                paging: 1,
                totalCount: count =>
            {
                RecommendationLearningPathTotalCount = count;
            });

            _recommendationLearningPathsCollection = learningPaths;
        }

        private void OnRefreshing(object obj)
        {
            InitLearningPaths();

            IsRefreshing = false;
        }

        private void CreateNewLearningPaths(object obj)
        {
            _navigationService.NavigateToAsync<MyLearningPathsCreateNewViewModel>();
        }

        private async Task OnSearchLearningPath()
        {
            if (string.IsNullOrEmpty(SearchLearningPathKeyword))
            {
                return;
            }

            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            using (DialogService.DisplayLoadingIndicator())
            {
                _currentSearchFilterGroup = LearningPathSearchFilter.Owner.ToString();
                SearchCollection = new LearnerObservableCollection<LearningPath>();
                _learningPathFromLMMPaging = 1;

                await SearchLearningPathFromLMM(totalCount: async searchRecommendationCount =>
                {
                    var learningPathInCurrentSearchFilterGroup = await SearchLearningPathFromLearner(true, statistic =>
                    {
                        // Create filter statistics
                        var filterStatistics = new Dictionary<string, int>
                        {
                            { LearningPathSearchFilter.Owner.ToString(), statistic[LearningPathSearchFilter.Owner.ToString()] },
                            { LearningPathSearchFilter.Shared.ToString(), statistic[LearningPathSearchFilter.Shared.ToString()] },
                            { LearningPathSearchFilter.Recommendations.ToString(), searchRecommendationCount }
                        };

                        // Init filter data
                        FilterData = new SearchGroupFilterData
                        {
                            Data = filterStatistics,
                            CurrentFilter = _currentSearchFilterGroup,
                            Keyword = SearchLearningPathKeyword
                        };

                        // Total of current filter
                        SearchFilterTotalCount = filterStatistics[_currentSearchFilterGroup];
                    });

                    SearchCollection.AddRange(learningPathInCurrentSearchFilterGroup);
                    RaisePropertyChanged(() => SearchCollection);
                });

                IsVisibleLearningSearch = true;
            }

            IsBusy = false;
        }

        private async Task OnSearchLearningPathFilter(Dictionary<string, int> currentFilter)
        {
            _currentSearchFilterGroup = currentFilter.FirstOrDefault().Key;
            SearchFilterTotalCount = currentFilter[_currentSearchFilterGroup];

            if (SearchFilterTotalCount == 0)
            {
                return;
            }

            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            _learningPathFromLMMPaging = 1;
            await LoadLearningPathData();

            IsVisibleLearningSearch = true;

            IsBusy = false;
        }

        private async Task<List<LearningPath>> SearchLearningPathFromLearner(bool isIncludeStatistic, Action<Dictionary<string, int>> statistic = null)
        {
            var searchResult = await ExecuteBackendService(() =>
            _learnerBackendService.SearchLearningPath(new
            {
                SearchText = SearchLearningPathKeyword,
                IncludeStatistic = isIncludeStatistic,
                StatusFilter = _currentSearchFilterGroup,
                StatisticsFilter = new string[] { LearningPathSearchFilter.Owner.ToString(), LearningPathSearchFilter.Shared.ToString() },
                SkipCount = SearchCollection.IsNullOrEmpty() ? 0 : SearchCollection.Count,
                MaxResultCount = GlobalSettings.MaxResultPerPage
            }));

            if (searchResult.HasEmptyResult() || searchResult.IsError)
            {
                return null;
            }

            if (isIncludeStatistic)
            {
                statistic?.Invoke(searchResult.Payload.Statistics.ToDictionary(p => p.Type, p => p.TotalCount));
            }

            return searchResult.Payload.Items;
        }

        private async Task<List<LearningPath>> SearchLearningPathFromLMM(Action<int> totalCount = null)
        {
            var learningPaths = await _commonService.RecommendationLearningPathsCollection(
                    userId: Application.Current.Properties.GetAccountProperties().User.Sub,
                    paging: _learningPathFromLMMPaging,
                    searchText: TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(SearchLearningPathKeyword),
                    totalCount: count =>
             {
                 totalCount?.Invoke(count);
             });

            return learningPaths;
        }

        private async Task OnSearchLearningPathRefresh()
        {
            await LoadLearningPathData();
        }

        private async Task LoadLearningPathData()
        {
            SearchCollection = new LearnerObservableCollection<LearningPath>();
            List<LearningPath> searchLearningPath = null;
            if (_currentSearchFilterGroup.Equals(LearningPathSearchFilter.Recommendations.ToString()))
            {
                searchLearningPath = await SearchLearningPathFromLMM(totalCount: currentFilterTotalCount =>
                {
                    SearchFilterTotalCount = currentFilterTotalCount;
                });
            }
            else
            {
                searchLearningPath = await SearchLearningPathFromLearner(false, statistic =>
                {
                    SearchFilterTotalCount = statistic[_currentSearchFilterGroup];
                });
            }

            SearchCollection.AddRange(searchLearningPath);
            RaisePropertyChanged(() => SearchCollection);
        }

        private async Task OnLoadmoreSearchLearningPath()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            List<LearningPath> loadmoreLearningPath = null;
            _learningPathFromLMMPaging++;

            if (_currentSearchFilterGroup.Equals(LearningPathSearchFilter.Recommendations.ToString()))
            {
                loadmoreLearningPath = await SearchLearningPathFromLMM();
            }
            else
            {
                loadmoreLearningPath = await SearchLearningPathFromLearner(false);
            }

            SearchCollection.AddRange(loadmoreLearningPath);

            IsBusy = false;
        }

        private void OnDismissSearch()
        {
            IsVisibleLearningSearch = false;
            SearchLearningPathKeyword = string.Empty;
        }
    }
}
