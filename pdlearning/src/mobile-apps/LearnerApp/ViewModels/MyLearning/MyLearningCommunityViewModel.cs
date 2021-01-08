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
using LearnerApp.Models.Learner;
using LearnerApp.Models.PdCatelogue;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningCommunityViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommunityBackendService _communityBackendService;
        private readonly IPdCatelogueService _pdCatelogueBackendService;
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        private string _communityListStatus;
        private string _keyword;
        private string _userId;
        private int _totalCount;
        private int _totalSearchCount;
        private int _paging;
        private int _searchPaging;
        private bool _isEnableSearch;
        private SearchGroupFilterData _filterData;
        private ResourceTypesFilter _currentFilterStatus = ResourceTypesFilter.All;

        public MyLearningCommunityViewModel()
        {
            _dialogService = DependencyService.Resolve<IDialogService>();
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _pdCatelogueBackendService = CreateRestClientFor<IPdCatelogueService>(GlobalSettings.BackendPdCatelogueService);
            _communityBackendService = CreateRestClientFor<ICommunityBackendService>(GlobalSettings.BackendServiceSocial);

            TotalCount = -1;
            TotalSearchCount = -1;
            var accountProperties = App.Current.Properties.GetAccountProperties();
            if (accountProperties != null)
            {
                _userId = accountProperties.User.Sub.ToLower();
            }
        }

        public ICommand RefreshCommand => new Command(async () => await OnRefresh());

        public ICommand LoadMoreItemCommand => new Command(async () => await LoadmoreItemsWithoutKeyword());

        public ICommand LoadmoreSearchCommand => new Command(async () => await LoadmoreSearchCommunities());

        public ICommand CommunityStatusSelectedCommand => new Command(CommunityStatusSelected);

        public ICommand SearchCommunityCommand => new Command(async () => await OnSearchCommunities());

        public ICommand BackSearchCommunityCommand => new Command(OnBackSearchCommunities);

        public ICommand SearchCommunityFilterCommand => new Command(async (currentFilterData) => await OnSearchCommunitiesByFilter(currentFilterData as Dictionary<string, int>));

        public LearnerObservableCollection<ItemCard> MyLearningCollection { get; set; }

        public LearnerObservableCollection<ItemCard> SearchCollection { get; set; }

        public string CommunityListStatus
        {
            get
            {
                return _communityListStatus;
            }

            set
            {
                _communityListStatus = value;
                RaisePropertyChanged(() => CommunityListStatus);
            }
        }

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

        public string Keyword
        {
            get
            {
                return _keyword;
            }

            set
            {
                _keyword = value;

                if (string.IsNullOrEmpty(_keyword))
                {
                    IsEnableSearch = false;
                }

                RaisePropertyChanged(() => Keyword);
            }
        }

        public int TotalCount
        {
            get
            {
                return _totalCount;
            }

            set
            {
                _totalCount = value;

                if (_totalCount != -1)
                {
                    RaisePropertyChanged(() => TotalCount);
                }
            }
        }

        public int TotalSearchCount
        {
            get
            {
                return _totalSearchCount;
            }

            set
            {
                _totalSearchCount = value;

                if (_totalSearchCount != -1)
                {
                    RaisePropertyChanged(() => TotalSearchCount);
                }
            }
        }

        public bool IsEnableSearch
        {
            get
            {
                return _isEnableSearch;
            }

            set
            {
                _isEnableSearch = value;
                RaisePropertyChanged(() => IsEnableSearch);
            }
        }

        public async Task GetCommunitiesWithoutKeyword(bool isLoadmore)
        {
            CommunityListStatus = SeparateStringByUppercase.Convert(_currentFilterStatus.ToString());

            if (!isLoadmore)
            {
                _paging = 1;
                MyLearningCollection = new LearnerObservableCollection<ItemCard>();
            }

            var searchParam = new PdCatelogueSearchFilter
            {
                Page = _paging,
                ResourceTypesFilter = new string[] { "community" },
                SearchCriteria = new PdSearchCriteria
                {
                    Status = new string[] { "contains", "enabled" },
                }
            };

            switch (_currentFilterStatus)
            {
                case ResourceTypesFilter.All:
                    searchParam.SearchCriteriaOr = new PdSearchCriteriaOr
                    {
                        CreatedBy = new string[] { "equals", _userId },
                        MembershipsId = new string[] { "contains", _userId }
                    };

                    break;
                case ResourceTypesFilter.MyCommunities:
                    searchParam.SearchCriteriaOr = new PdSearchCriteriaOr
                    {
                        MembershipsId = new string[] { "contains", _userId }
                    };

                    break;
                case ResourceTypesFilter.MyOwnCommunities:
                    searchParam.SearchCriteriaOr = new PdSearchCriteriaOr
                    {
                        CreatedBy = new string[] { "equals", _userId },
                    };

                    break;
            }

            var searchResult = await ExecuteBackendService(() => _pdCatelogueBackendService.Search(searchParam));

            if (!isLoadmore)
            {
                TotalCount = searchResult.Payload.Total;
            }

            if (!searchResult.HasEmptyResult() && !searchResult.IsError && searchResult.Payload.Resources.Any())
            {
                var communityIds = searchResult.Payload.Resources.Select(p => p.Id).ToArray();
                var communitiesResult = await ExecuteBackendService(() => _communityBackendService.GetCommunityByIds(new GetCommunityByIdRequestModel(communityIds)));

                if (!communitiesResult.HasEmptyResult() && !communitiesResult.IsError)
                {
                    var bookmarkedCommunitiesResult = await ExecuteBackendService(() => _learnerBackendService.GetBookmarkedByIds(communityIds, ResourceTypesFilter.Community.ToString()));

                    var communityCardList = CommunityCardBuilder.BuildCommunityCardListAsync(communitiesResult.Payload.Results, bookmarkedCommunitiesResult.Payload);

                    MyLearningCollection.AddRange(communityCardList);
                    if (!isLoadmore)
                    {
                        RaisePropertyChanged(() => MyLearningCollection);
                    }
                }

                bool isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
                if (!isVisibleBookmark)
                {
                    foreach (var item in MyLearningCollection)
                    {
                        item.IsVisibleBookmark = false;
                    }
                }
            }
        }

        private async Task OnSearchCommunities()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                if (string.IsNullOrEmpty(Keyword))
                {
                    return;
                }

                _currentFilterStatus = ResourceTypesFilter.MyCommunities;
                await GetSearchCommunities(false, true);

                IsEnableSearch = true;
            }
        }

        private async Task GetSearchCommunities(bool isLoadmore, bool isInitSearchFilterData = false)
        {
            if (!isLoadmore)
            {
                _searchPaging = 1;
                SearchCollection = new LearnerObservableCollection<ItemCard>();
            }

            var searchParam = new PdCatelogueSearchFilter
            {
                Page = _searchPaging,
                SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(Keyword),
                ResourceTypesFilter = new string[] { "community" },
                SearchCriteria = new PdSearchCriteria
                {
                    Status = new string[] { "contains", "enabled" },
                },
                SearchCriteriaOr = new PdSearchCriteriaOr
                {
                    CreatedBy = new string[] { "equals", _userId },
                    MembershipsId = new string[] { "contains", _userId }
                }
            };

            switch (_currentFilterStatus)
            {
                case ResourceTypesFilter.MyCommunities:
                    searchParam.StatisticResourceTypes = new string[] { "memberships.id", "createdby" };
                    break;
                case ResourceTypesFilter.MyOwnCommunities:
                    searchParam.StatisticResourceTypes = new string[] { "createdby", "memberships.id" };
                    break;
                default:
                    break;
            }

            var searchResult = await ExecuteBackendService(() => _pdCatelogueBackendService.Search(searchParam));

            if (!isLoadmore)
            {
                var statisticsValue = searchResult.Payload.ResourceStatistics.ToDictionary(p => p.Type.ToString(), p => p.Total);

                TotalSearchCount = searchResult.Payload.Total;

                if (isInitSearchFilterData)
                {
                    FilterData = new SearchGroupFilterData
                    {
                        Data = new Dictionary<string, int>
                            {
                                { ResourceTypesFilter.MyCommunities.ToString(), statisticsValue[ResourceTypesFilter.MyCommunities.ToString()] },
                                { ResourceTypesFilter.MyOwnCommunities.ToString(), statisticsValue[ResourceTypesFilter.MyOwnCommunities.ToString()] }
                            },
                        CurrentFilter = _currentFilterStatus.ToString(),
                        Keyword = Keyword
                    };
                }
            }

            if (!searchResult.HasEmptyResult() && searchResult.Payload.Resources.Any())
            {
                var itemIds = searchResult.Payload.Resources.Select(p => p.Id).ToArray();
                var communitiesResult = await ExecuteBackendService(() => _communityBackendService.GetCommunityByIds(new GetCommunityByIdRequestModel(itemIds)));

                if (!communitiesResult.HasEmptyResult() && !communitiesResult.IsError)
                {
                    var bookmarkedCommunitiesResult = await ExecuteBackendService(() => _learnerBackendService.GetBookmarkedByIds(itemIds, ResourceTypesFilter.Community.ToString()));

                    var communityCardList = CommunityCardBuilder.BuildCommunityCardListAsync(communitiesResult.Payload.Results, bookmarkedCommunitiesResult.Payload);

                    SearchCollection.AddRange(communityCardList);

                    if (!isLoadmore)
                    {
                        RaisePropertyChanged(() => SearchCollection);
                    }
                }

                bool isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
                if (!isVisibleBookmark)
                {
                    foreach (var item in MyLearningCollection)
                    {
                        item.IsVisibleBookmark = false;
                    }
                }
            }
        }

        private async Task LoadmoreSearchCommunities()
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                if (SearchCollection.Count == TotalSearchCount)
                {
                    return;
                }

                _searchPaging++;

                await GetSearchCommunities(true);
            });
        }

        private async Task LoadmoreItemsWithoutKeyword()
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                if (MyLearningCollection.Count == TotalCount)
                {
                    return;
                }

                _paging++;

                await GetCommunitiesWithoutKeyword(true);
            });
        }

        private void CommunityStatusSelected(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var items = new Dictionary<string, string>
                {
                    { ResourceTypesFilter.All.ToString(), null },
                    { ResourceTypesFilter.MyOwnCommunities.ToString(), null },
                    { ResourceTypesFilter.MyCommunities.ToString(), null }
                };

                _dialogService.ShowDropDownSelectionPopup(items, isFullScreen: false, onSelected: async currentMyLearningGroupSelected =>
                {
                    if (!string.IsNullOrEmpty(currentMyLearningGroupSelected))
                    {
                        Enum.TryParse(currentMyLearningGroupSelected, out ResourceTypesFilter currentStatus);

                        _currentFilterStatus = currentStatus;
                        await GetCommunitiesWithoutKeyword(false);
                    }
                });
            });
        }

        private async Task OnRefresh()
        {
            if (IsEnableSearch)
            {
                await GetSearchCommunities(false, true);
            }
            else
            {
                await GetCommunitiesWithoutKeyword(false);
            }
        }

        private async Task OnSearchCommunitiesByFilter(Dictionary<string, int> currentFilterData)
        {
            Enum.TryParse(currentFilterData.FirstOrDefault().Key, out ResourceTypesFilter currentFilter);

            _currentFilterStatus = currentFilter;
            await GetSearchCommunities(false);
        }

        private void OnBackSearchCommunities(object obj)
        {
            Keyword = string.Empty;
            IsEnableSearch = false;
        }
    }
}
