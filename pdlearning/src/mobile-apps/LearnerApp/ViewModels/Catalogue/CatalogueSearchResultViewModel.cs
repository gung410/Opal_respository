using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.Search;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.DataManager.CatalogueFilter;
using LearnerApp.ViewModels.Base;
using LearnerApp.Views;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Catalogue
{
    public class CatalogueSearchResultViewModel : BaseViewModel
    {
        private readonly IPdCatelogueService _pdCatelogueService;
        private readonly ICommonServices _commonService;

        private int _paging = 1;
        private int _searchTotalCount;
        private string _keyword;
        private bool _isEnableSearch;
        private bool _isSearchPdo;
        private CatalogueFilterModel _searchFilter;
        private ResourceTypesFilter _currentSearchFilter;
        private SearchGroupFilterData _filterData;

        private StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public CatalogueSearchResultViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _pdCatelogueService = CreateRestClientFor<IPdCatelogueService>(GlobalSettings.BackendPdCatelogueService);

            _currentSearchFilter = ResourceTypesFilter.All;
            IsSearchPdo = true;

            MessagingCenter.Subscribe<AdvancedSearchView, CatalogueFilterModel>(this, "advanced-search-filter-value", async (sender, args) =>
            {
                if (!args.IsEmpty() || !string.IsNullOrEmpty(Keyword))
                {
                    _searchFilter = args;
                    await OnCatalogueSearch();

                    // Handle in-visible session on catalogue page when advandce search actived.
                    MessagingCenter.Send(this, "advanced-search-filter-result");
                }
            });
        }

        ~CatalogueSearchResultViewModel()
        {
            MessagingCenter.Unsubscribe<AdvancedSearchView, CatalogueFilterModel>(this, "advanced-search-filter-value");
        }

        public LearnerObservableCollection<ItemCard> SearchCollection { get; set; }

        public ICommand BackSearchCommand => new Command(DismissSearchView);

        public ICommand SearchFilterCommand => new Command(async (currentFilter) => await OnSearchFilter(currentFilter as Dictionary<string, int>));

        public ICommand LoadmoreCatalogueSearchCommand => new Command(async () => await OnLoadmoreCatalogueSearch());

        public ICommand RefreshCatalogueSearchCommand => new Command(async () =>
        {
            if (!string.IsNullOrEmpty(Keyword))
            {
                await OnCatalogueSearch();
            }
        });

        public ICommand CatalogueSearchCommand => new Command(async () =>
        {
            if (!string.IsNullOrEmpty(Keyword))
            {
                await OnCatalogueSearch();
            }
        });

        public ICommand AdvancedSearchCommand => new Command(OnAdvancedSearch);

        public ICommand PdoTappedCommand => new Command(OnPdoTapped);

        public ICommand UserSearchCommand => new Command(OnUserSearchTapped);

        public string Keyword
        {
            get
            {
                return _keyword;
            }

            set
            {
                _keyword = value;

                if (string.IsNullOrEmpty(value))
                {
                    IsEnableSearch = false;
                }

                RaisePropertyChanged(() => Keyword);
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

        public bool IsSearchPdo
        {
            get
            {
                return _isSearchPdo;
            }

            set
            {
                _isSearchPdo = value;
                RaisePropertyChanged(() => IsSearchPdo);
            }
        }

        public int SearchTotalCount
        {
            get
            {
                return _searchTotalCount;
            }

            set
            {
                _searchTotalCount = value;

                RaisePropertyChanged(() => SearchTotalCount);
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

        private void DismissSearchView(object obj)
        {
            IsEnableSearch = false;
            Keyword = string.Empty;
        }

        private async Task OnSearchFilter(Dictionary<string, int> currentFilter)
        {
            SearchCollection = new LearnerObservableCollection<ItemCard>();
            _paging = 1;

            Enum.TryParse(currentFilter.FirstOrDefault().Key, out ResourceTypesFilter currentStatus);
            _currentSearchFilter = currentStatus;

            await LoadSearchItemsAsync(false);
        }

        private async Task OnLoadmoreCatalogueSearch()
        {
            if (SearchCollection.Count < SearchTotalCount)
            {
                _paging++;

                await LoadSearchItemsAsync(false);
            }
        }

        private async Task OnCatalogueSearch()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                _paging = 1;
                SearchCollection = new LearnerObservableCollection<ItemCard>();

                await LoadSearchItemsAsync(true);

                IsEnableSearch = true;
            }
        }

        private async Task LoadSearchItemsAsync(bool isInitFilterData)
        {
            var communityFilters = new List<SearchQueryFieldBuilderModel>()
            {
                new SearchQueryFieldBuilderModel("tags.id", "equals", _searchFilter?.GetTagsIds())
            };

            if (_searchFilter != null && !_searchFilter.CommunityTypes.IsNullOrEmpty())
            {
                communityFilters.Add(new SearchQueryFieldBuilderModel(
                    "JoinPolicyType",
                    "equals",
                    _searchFilter.CommunityTypes.Select(x => x.ToApiString()).ToArray()));
            }

            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var searchParam = new
                {
                    Page = _paging,
                    Limit = GlobalSettings.MaxResultPerPage,
                    SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(Keyword),
                    ResourceType = _currentSearchFilter.ToString(),
                    EnableStatistics = isInitFilterData,
                    AttachmentTypeFilter = _searchFilter?.AttachmentType?.ToApiString(),
                    JobDesignation = _searchFilter?.JobDesignation?.Id,
                    ResourceStatusFilters = new
                    {
                        Community = new string[] { "enabled", "restricted" },
                        Course = new string[] { "published" },
                        Content = new string[] { "published" },
                        Form = new string[] { "published" },
                        Microlearning = new string[] { "published" },
                        LearningPath = new string[] { "published" },
                    },
                    StatisticResourceTypes = new string[]
                    {
                        "content",
                        "microlearning",
                        "course",
                        "learningpath",
                        "community",
                        "form"
                    },
                    SearchFields = new string[]
                    {
                        "title",
                        "description",
                        "code",
                        "externalcode",
                        "tag"
                    },
                    UseFuzzy = true,
                    UseSynonym = true,
                    Filters = new
                    {
                        Content = new[]
                        {
                            new { FieldName = "IsArchived", Operator = "equals", Values = new string[] { "false" } },
                            new { FieldName = "startDate", Operator = "lte", Values = new string[] { DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") } },
                            new { FieldName = "expiredDate", Operator = "gt", Values = new string[] { DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") } },
                            new { FieldName = "tags.id", Operator = "equals", Values = _searchFilter?.GetTagsIds() }
                        },
                        Course = new[]
                        {
                            new { FieldName = "registrationMethod", Operator = "contains", Values = new string[] { "public", "restricted" } },
                            new { FieldName = "tags.id", Operator = "equals", Values = _searchFilter?.GetTagsIds() }
                        },
                        Microlearning = new[]
                        {
                            new { FieldName = "tags.id", Operator = "equals", Values = _searchFilter?.GetTagsIds() }
                        },
                        LearningPath = new[]
                        {
                            new { FieldName = "tags.id", Operator = "equals", Values = _searchFilter?.GetTagsIds() }
                        },
                        Community = communityFilters,
                        Form = new[]
                        {
                            new { FieldName = "tags.id", Operator = "equals", Values = _searchFilter?.GetTagsIds() }
                        }
                    }
                };

                var searchCourses = await ExecuteBackendService(() => _pdCatelogueService.SearchV2(searchParam));

                if (!searchCourses.HasEmptyResult())
                {
                    SearchTotalCount = searchCourses.Payload.Total;

                    if (isInitFilterData)
                    {
                        FilterData = new SearchGroupFilterData
                        {
                            Data = new Dictionary<string, int>
                            {
                                { ResourceTypesFilter.All.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.All).Total },
                                { ResourceTypesFilter.Course.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.Course).Total },
                                { ResourceTypesFilter.Content.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.Content).Total },
                                { ResourceTypesFilter.Microlearning.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.Microlearning).Total },
                                { ResourceTypesFilter.LearningPath.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.LearningPath).Total },
                                { ResourceTypesFilter.Community.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.Community).Total },
                                { ResourceTypesFilter.Form.ToString(), searchCourses.Payload.ResourceStatistics.FirstOrDefault(p => p.Type == ResourceTypesFilter.Form).Total }
                            },
                            CurrentFilter = _currentSearchFilter.ToString(),
                            Keyword = Keyword
                        };
                    }

                    var resourceTypeGroup = searchCourses.Payload.Resources.GroupBy(p => p.ResourceType);
                    var courseCard = await _commonService.GetItemCardByGroup(resourceTypeGroup);

                    SearchCollection.AddRange(courseCard);
                    RaisePropertyChanged(() => SearchCollection);
                }
            });
        }

        private void OnAdvancedSearch(object obj)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                _searchFilter = null;

                DialogService.ShowAdvancedSearch();
            }
        }

        private void OnUserSearchTapped()
        {
            IsSearchPdo = false;
        }

        private void OnPdoTapped()
        {
            IsSearchPdo = true;
        }
    }
}
