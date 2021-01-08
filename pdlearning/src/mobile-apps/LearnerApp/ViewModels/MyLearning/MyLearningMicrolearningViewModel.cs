using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningMicrolearningViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly ICommonServices _commonService;

        private MyLearningStatus _currentMyLearningStatus;
        private SearchGroupFilterData _filterData;
        private string _courseListStatus;
        private string _searchMicrolearningKeyword;
        private string _searchMicrolearningFilterGroup;
        private int _totalCount;
        private int _searchMicrolearningFilterTotalCount;
        private bool _isEnableMicrolearningSearch;

        public MyLearningMicrolearningViewModel()
        {
            TotalCount = -1;

            _dialogService = DependencyService.Resolve<IDialogService>();
            _commonService = DependencyService.Resolve<ICommonServices>();

            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);

            CourseBookmarkMessagingCenter.Subscribe(this, async (sender, args) =>
            {
                await Refresh();
            });
        }

        public ICommand LoadMoreItemCommand => new Command(async () => await LoadMoreItemsAsync());

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand SearchMicrolearningFilterRefreshCommand => new Command(async () => await OnSearchMicrolearning());

        public ICommand SearchMicrolearningCommand => new Command(async () => await OnSearchMicrolearning());

        public ICommand SearchMicrolearningFilterCommand => new Command(async (currentFilterData) => await OnSearchMicrolearningByFilter(currentFilterData as Dictionary<string, int>));

        public ICommand LoadMoreSearchMicrolearningFilterItemCommand => new Command(async () => await OnSearchMicrolearningByFilter(false));

        public ICommand BackSearchMicrolearningCommand => new Command(OnBackSearchMicrolearning);

        public ICommand CourseStatusSelectedCommand => new Command(CourseStatusSelected);

        public LearnerObservableCollection<ItemCard> MicrolearningCollection { get; set; }

        public LearnerObservableCollection<ItemCard> MySearchMicrolearningFilterCollection { get; set; }

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

        public int SearchMicrolearningFilterTotalCount
        {
            get
            {
                return _searchMicrolearningFilterTotalCount;
            }

            set
            {
                _searchMicrolearningFilterTotalCount = value;
                RaisePropertyChanged(() => SearchMicrolearningFilterTotalCount);
            }
        }

        public bool IsEnableMicrolearningSearch
        {
            get
            {
                return _isEnableMicrolearningSearch;
            }

            set
            {
                _isEnableMicrolearningSearch = value;
                RaisePropertyChanged(() => IsEnableMicrolearningSearch);
            }
        }

        public string SearchMicrolearningKeyword
        {
            get
            {
                return _searchMicrolearningKeyword;
            }

            set
            {
                _searchMicrolearningKeyword = value;

                if (string.IsNullOrEmpty(_searchMicrolearningKeyword))
                {
                    IsEnableMicrolearningSearch = false;
                }

                RaisePropertyChanged(() => SearchMicrolearningKeyword);
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

                if (value != -1)
                {
                    RaisePropertyChanged(() => TotalCount);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            CourseBookmarkMessagingCenter.Unsubscribe(this);
        }

        public async Task InitScreen(MyLearningStatus myStatus)
        {
            MicrolearningCollection = new LearnerObservableCollection<ItemCard>();

            _currentMyLearningStatus = myStatus;

            var moreCourseCards = await GetMicrolearningCardByLearningStatus(myStatus);

            MicrolearningCollection.AddRange(moreCourseCards);

            RaisePropertyChanged(() => MicrolearningCollection);
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

        private async Task<List<ItemCard>> GetMicrolearningCardByLearningStatus(MyLearningStatus myStatus)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                ApiResponse<ListResultDto<MyCourseSummary>> microlearningSummaryList = null;

                microlearningSummaryList = await ExecuteBackendService(() => _learnerBackendService.GetMyLearningCourse(courseType: "Microlearning", statusFilter: myStatus, orderBy: "LastLogin DESC", skipCount: MicrolearningCollection.Count));

                if (!microlearningSummaryList.HasEmptyResult())
                {
                    var myCourseList = microlearningSummaryList.Payload.Items;

                    TotalCount = microlearningSummaryList.Payload.TotalCount;
                    CourseListStatus = $"{SeparateStringByUppercase.Convert(_currentMyLearningStatus.ToString())} ({TotalCount})";

                    if (TotalCount == 0)
                    {
                        return null;
                    }

                    var myCourseIdentifiers = myCourseList.Select(c => c.CourseId).ToArray();

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

        private async Task LoadMoreItemsAsync()
        {
            var moreCourseCards = await GetMicrolearningCardByLearningStatus(_currentMyLearningStatus);

            // Load more -> add more items to the list.
            MicrolearningCollection.AddRange(moreCourseCards);

            RaisePropertyChanged(() => MicrolearningCollection);
        }

        private async Task Refresh()
        {
            await InitScreen(_currentMyLearningStatus);
        }

        private void OnBackSearchMicrolearning(object obj)
        {
            IsEnableMicrolearningSearch = false;
            SearchMicrolearningKeyword = string.Empty;
        }

        private async Task OnSearchMicrolearningByFilter(Dictionary<string, int> currentFilterData)
        {
            MySearchMicrolearningFilterCollection = new LearnerObservableCollection<ItemCard>();

            _searchMicrolearningFilterGroup = currentFilterData.FirstOrDefault().Key;
            SearchMicrolearningFilterTotalCount = currentFilterData[_searchMicrolearningFilterGroup];

            if (SearchMicrolearningFilterTotalCount == 0)
            {
                return;
            }

            await OnSearchMicrolearningByFilter(false);
        }

        private async Task OnSearchMicrolearning()
        {
            if (string.IsNullOrEmpty(SearchMicrolearningKeyword))
            {
                return;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                MySearchMicrolearningFilterCollection = new LearnerObservableCollection<ItemCard>();

                _searchMicrolearningFilterGroup = MyLearningStatus.InProgress.ToString();

                await OnSearchMicrolearningByFilter(true);
            }
        }

        private async Task OnSearchMicrolearningByFilter(bool isInitGroupFilter)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var searchResult = await ExecuteBackendService(() =>
            _learnerBackendService.MyLearningCourseSearch(new
            {
                SearchText = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(SearchMicrolearningKeyword),
                IncludeStatistic = isInitGroupFilter,
                CourseType = LearningCourseType.Microlearning.ToString(),
                StatusFilter = _searchMicrolearningFilterGroup,
                StatisticsFilter = new string[] { MyLearningStatus.InProgress.ToString(), MyLearningStatus.Completed.ToString() },
                SkipCount = MySearchMicrolearningFilterCollection == null ? 0 : MySearchMicrolearningFilterCollection.Count,
                MaxResultCount = GlobalSettings.MaxResultPerPage
            }));

            if (searchResult.HasEmptyResult() || searchResult.IsError)
            {
                IsBusy = false;

                return;
            }

            if (isInitGroupFilter)
            {
                SearchMicrolearningFilterTotalCount = searchResult.Payload.Statistics.FirstOrDefault(p => p.Type == _searchMicrolearningFilterGroup).TotalCount;

                FilterData = new SearchGroupFilterData
                {
                    Data = searchResult.Payload.Statistics.ToDictionary(p => p.Type, p => p.TotalCount),
                    CurrentFilter = _searchMicrolearningFilterGroup,
                    Keyword = SearchMicrolearningKeyword
                };
            }

            // Get course card by course summary
            var courseCard = await _commonService.BuildCourseCardList(searchResult.Payload.Items);
            if (!courseCard.IsNullOrEmpty())
            {
                MySearchMicrolearningFilterCollection.AddRange(courseCard);
            }

            RaisePropertyChanged(() => MySearchMicrolearningFilterCollection);

            IsEnableMicrolearningSearch = true;

            IsBusy = false;
        }
    }
}
