using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MyLearningCourseViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ICommonServices _commonService;

        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICourseBackendService _courseBackendService;

        private MyLearningStatus _currentMyLearningStatus;
        private SearchGroupFilterData _filterData;
        private string _courseListStatus;
        private string _searchCourseFilterKeyword;
        private string _currentSearchFilterGroup;
        private int _totalCount;
        private int _searchCourseFilterTotalCount;
        private bool _isEnableCourseSearch;

        public MyLearningCourseViewModel()
        {
            _dialogService = DependencyService.Resolve<IDialogService>();
            _commonService = DependencyService.Resolve<ICommonServices>();

            SearchCourseFilterKeyword = string.Empty;
            _currentSearchFilterGroup = MyLearningStatus.Registered.ToString();

            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
        }

        public ICommand LoadMoreItemCommand => new Command(async () => await LoadMoreItemsAsync());

        public ICommand LoadMoreSearchCourseFilterItemCommand => new Command(async () => await SearchCourseByFilter(false));

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand SearchCourseFilterRefreshCommand => new Command(async () => await OnSearchCourse());

        public ICommand CourseStatusSelectedCommand => new Command(CourseStatusSelected);

        public ICommand SearchCourseCommand => new Command(async () => await OnSearchCourse());

        public ICommand SearchCourseFilterCommand => new Command(async (currentFilterData) => await OnSearchCourseByFilter(currentFilterData as Dictionary<string, int>));

        public ICommand BackSearchCourseCommand => new Command(OnBackSearchCourse);

        public LearnerObservableCollection<ItemCard> MyLearningCollection { get; set; }

        public LearnerObservableCollection<ItemCard> MySearchCourseFilterCollection { get; set; }

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

        public bool IsEnableCourseSearch
        {
            get
            {
                return _isEnableCourseSearch;
            }

            set
            {
                _isEnableCourseSearch = value;
                RaisePropertyChanged(() => IsEnableCourseSearch);
            }
        }

        public string SearchCourseFilterKeyword
        {
            get
            {
                return _searchCourseFilterKeyword;
            }

            set
            {
                _searchCourseFilterKeyword = value;

                if (string.IsNullOrEmpty(_searchCourseFilterKeyword))
                {
                    IsEnableCourseSearch = false;
                }

                RaisePropertyChanged(() => SearchCourseFilterKeyword);
            }
        }

        public string CourseListStatus
        {
            get
            {
                return _courseListStatus;
            }

            set
            {
                _courseListStatus = value;

                RaisePropertyChanged(() => CourseListStatus);
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
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public int SearchCourseFilterTotalCount
        {
            get
            {
                return _searchCourseFilterTotalCount;
            }

            set
            {
                _searchCourseFilterTotalCount = value;
                RaisePropertyChanged(() => SearchCourseFilterTotalCount);
            }
        }

        public async Task InitScreen(MyLearningStatus myLearningStatus)
        {
            MyLearningCollection = new LearnerObservableCollection<ItemCard>();

            _currentMyLearningStatus = myLearningStatus;

            var moreCourseCards = await GetCourseCardByMyLearningStatus(myLearningStatus);

            // Init screen, remove all and add new items.
            MyLearningCollection.AddRange(moreCourseCards);
            RaisePropertyChanged(() => MyLearningCollection);
        }

        private async Task Refresh()
        {
            await InitScreen(_currentMyLearningStatus);
        }

        private async Task LoadMoreItemsAsync()
        {
            var moreCourseCards = await GetCourseCardByMyLearningStatus(_currentMyLearningStatus);

            // Load more -> add more items to the list.
            MyLearningCollection.AddRange(moreCourseCards);
        }

        private async Task<List<ItemCard>> GetCourseCardByMyLearningStatus(MyLearningStatus myLearningStatus)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var myCourseSummaryList = await ExecuteBackendService(() => _learnerBackendService.GetMyLearningCourse(
                    courseType: "FaceToFace",
                    statusFilter: myLearningStatus,
                    orderBy: myLearningStatus == MyLearningStatus.Completed ? "CompletedDate DESC" : "LastLogin DESC",
                    skipCount: MyLearningCollection.Count));

                if (!myCourseSummaryList.HasEmptyResult())
                {
                    var myCourseList = myCourseSummaryList.Payload.Items;

                    TotalCount = myCourseSummaryList.Payload.TotalCount;
                    CourseListStatus = $"{SeparateStringByUppercase.Convert(_currentMyLearningStatus.ToString())} ({TotalCount})";

                    if (TotalCount == 0)
                    {
                        return null;
                    }

                    var myCourseIdentifiers = myCourseList.Select(c => c.CourseId).ToArray();

                    // From my course identifiers, we need to get more data about a course by calling course API.
                    var courseListApiResult = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(myCourseIdentifiers));

                    if (courseListApiResult.HasEmptyResult() || courseListApiResult.Payload.IsNullOrEmpty())
                    {
                        return null;
                    }

                    var classRunList = await _commonService.GetListMyClassRunDetails(myCourseList);

                    // Get current lecture name
                    var myCurrentLectureName = await _commonService.GetCurrentLectureNameByIds(myCourseList);

                    var courseCardList = CourseCardBuilder.BuildCourseCardListAsync(myCourseList, courseListApiResult.Payload, classRunList, myCurrentLectureName);

                    return courseCardList;
                }

                return null;
            }
        }

        private void CourseStatusSelected(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!IsBusy)
                {
                    IsBusy = true;

                    var items = new Dictionary<string, string>
                    {
                        { MyLearningStatus.Registered.ToString(), null },
                        { MyLearningStatus.Upcoming.ToString(), null },
                        { MyLearningStatus.InProgress.ToString(), null },
                        { MyLearningStatus.Completed.ToString(), null }
                    };

                    _dialogService.ShowDropDownSelectionPopup(items, isFullScreen: false, onSelected: async currentMyLearningGroupSelected =>
                    {
                        if (!string.IsNullOrEmpty(currentMyLearningGroupSelected))
                        {
                            Enum.TryParse(currentMyLearningGroupSelected, out MyLearningStatus myStatus);

                            await InitScreen(myStatus);
                        }
                    });
                }

                IsBusy = false;
            });
        }

        private void OnBackSearchCourse(object obj)
        {
            IsEnableCourseSearch = false;
            SearchCourseFilterKeyword = string.Empty;
        }

        private async Task OnSearchCourse()
        {
            if (string.IsNullOrEmpty(SearchCourseFilterKeyword))
            {
                return;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                MySearchCourseFilterCollection = new LearnerObservableCollection<ItemCard>();

                await SearchCourseByFilter(true);
            }
        }

        private async Task OnSearchCourseByFilter(Dictionary<string, int> currentFilterData)
        {
            MySearchCourseFilterCollection = new LearnerObservableCollection<ItemCard>();

            _currentSearchFilterGroup = currentFilterData.FirstOrDefault().Key;
            SearchCourseFilterTotalCount = currentFilterData[_currentSearchFilterGroup];

            if (SearchCourseFilterTotalCount == 0)
            {
                return;
            }

            await SearchCourseByFilter(false);
        }

        private async Task SearchCourseByFilter(bool isInitGroupFilter)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var searchResult = await ExecuteBackendService(() =>
            _learnerBackendService.MyLearningCourseSearch(new
            {
                SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(SearchCourseFilterKeyword),
                IncludeStatistic = isInitGroupFilter,
                CourseType = LearningCourseType.FaceToFace.ToString(),
                StatusFilter = _currentSearchFilterGroup,
                StatisticsFilter = new string[] { MyLearningStatus.Registered.ToString(), MyLearningStatus.Upcoming.ToString(), MyLearningStatus.InProgress.ToString(), MyLearningStatus.Completed.ToString() },
                SkipCount = MySearchCourseFilterCollection.IsNullOrEmpty() ? 0 : MySearchCourseFilterCollection.Count,
                MaxResultCount = GlobalSettings.MaxResultPerPage
            }));

            if (searchResult.HasEmptyResult() || searchResult.IsError)
            {
                IsBusy = false;

                return;
            }

            if (isInitGroupFilter)
            {
                SearchCourseFilterTotalCount = searchResult.Payload.Statistics.FirstOrDefault(p => p.Type == _currentSearchFilterGroup).TotalCount;

                FilterData = new SearchGroupFilterData
                {
                    Data = searchResult.Payload.Statistics.ToDictionary(p => p.Type, p => p.TotalCount),
                    CurrentFilter = _currentSearchFilterGroup,
                    Keyword = SearchCourseFilterKeyword
                };
            }

            // Get course card by course summary
            var courseCard = await _commonService.BuildCourseCardList(searchResult.Payload.Items);
            if (!courseCard.IsNullOrEmpty())
            {
                MySearchCourseFilterCollection.AddRange(courseCard);
            }

            RaisePropertyChanged(() => MySearchCourseFilterCollection);

            IsEnableCourseSearch = true;

            IsBusy = false;
        }
    }
}
