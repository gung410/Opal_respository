using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Common.TaskController;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Controls.MyLearning;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.Newsfeed;
using LearnerApp.Models.Permission;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Calendar;
using LearnerApp.ViewModels.Home.OutstandingTasks;
using LearnerApp.ViewModels.Sharing.Home;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class HomeViewModel : BasePageViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly ICommonServices _commonService;

        private string _name;
        private string _userId;
        private string _numberOfCompleteCourses;
        private string _numberOfOngoingCourses;
        private bool _isRefreshing = true;
        private bool _isShowMyLearningWidget;
        private bool _isShowRecommendForYouWidget;
        private bool _isShowRecommendForOrgWidget;
        private bool _isShowBookmarkWidget;
        private bool _isShowNewsfeedWidget;
        private bool _isVisibleSetting = true;
        private int _myLearningTotalCount;
        private int _recommendationsTotalCount;
        private int _recommendationsOrgTotalCount;
        private int _bookmarkTotalCount;
        private int _newsfeedTotalCount;
        private List<ItemCard> _myBookmarkCourseCardList;

        public HomeViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _commonService = DependencyService.Resolve<ICommonServices>();

            OutstandingTasksSection = new OutstandingTasksSectionViewModel(this);
            SharingSection = new SharingSectionViewModel();
            CalendarSection = new CalendarSectionViewModel();

            MessagingCenter.Subscribe<ListViewModel, ItemCard>(this, "bookmarked-item-on-list", async (sender, args) =>
            {
                await AddItemToMyBookmarkCollection(args);
            });

            CourseBookmarkMessagingCenter.Subscribe(this, async (sender, args) =>
            {
                var courseCard = _myBookmarkCourseCardList.FirstOrDefault(x => x.Id == args.CourseId);
                if (courseCard == null)
                {
                    return;
                }

                await AddItemToMyBookmarkCollection(courseCard);
            });

            MyDigitalContentBookmarkMessagingCenter.Subscribe(this, async (sender, args) =>
            {
                var courseCard = _myBookmarkCourseCardList.FirstOrDefault(x => x.Id == args.DigitalContentId || x.CardType == BookmarkType.DigitalContent);
                if (courseCard == null)
                {
                    return;
                }

                await AddItemToMyBookmarkCollection(courseCard);
            });

            MessagingCenter.Subscribe<MyLearningDigitalContent, ItemCard>(this, "bookmarked-item-on-list", async (sender, args) =>
            {
                await AddItemToMyBookmarkCollection(args);
            });

            MessagingCenter.Subscribe<MyLearningDigitalContentBookmarked, ItemCard>(this, "bookmarked-item-on-list", async (sender, args) =>
            {
                await AddItemToMyBookmarkCollection(args);
            });

            MessagingCenter.Subscribe<WidgetSettingViewModel>(this, "home-widget-configs", async (sender) =>
            {
                await LoadWidgetConfigs();
                await InitScreen();
            });
        }

        public Dictionary<string, bool> WidgetConfigs { get; private set; }

        public ICommand RefreshCommand => new Command(async () => await OnRefresh());

        public ICommand SettingCommand => new Command(async () => await GoToSetting());

        public ICommand LoadmoreNewsfeedCommand => new Command(OnSeemoreNewsfeed);

        public LearnerObservableCollection<ItemCard> MyLearningCollection { get; set; }

        public LearnerObservableCollection<ItemCard> MyRecommendationsCollection { get; set; }

        public LearnerObservableCollection<ItemCard> MyBookmarkCollection { get; set; }

        public LearnerObservableCollection<ItemCard> MyRecommendationOrganizationCollection { get; set; }

        public LearnerObservableCollection<Feed> NewsfeedCollection { get; set; }

        public OutstandingTasksSectionViewModel OutstandingTasksSection { get; set; }

        public SharingSectionViewModel SharingSection { get; set; }

        public CalendarSectionViewModel CalendarSection { get; set; }

        public int MyLearningTotalCount
        {
            get
            {
                return _myLearningTotalCount;
            }

            set
            {
                _myLearningTotalCount = value;
                RaisePropertyChanged(() => MyLearningTotalCount);
            }
        }

        public int NewsfeedTotalCount
        {
            get
            {
                return _newsfeedTotalCount;
            }

            set
            {
                _newsfeedTotalCount = value;
                RaisePropertyChanged(() => NewsfeedTotalCount);
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

        public int RecommendationsOrgTotalCount
        {
            get
            {
                return _recommendationsOrgTotalCount;
            }

            set
            {
                _recommendationsOrgTotalCount = value;
                RaisePropertyChanged(() => RecommendationsOrgTotalCount);
            }
        }

        public int BookmarkTotalCount
        {
            get
            {
                return _bookmarkTotalCount;
            }

            set
            {
                _bookmarkTotalCount = value;
                RaisePropertyChanged(() => BookmarkTotalCount);
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
                IsRefreshingForRefreshView = value;
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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string NumberOfCompleteCourses
        {
            get
            {
                return _numberOfCompleteCourses;
            }

            set
            {
                _numberOfCompleteCourses = value;
                RaisePropertyChanged(() => NumberOfCompleteCourses);
            }
        }

        public string NumberOfOngoingCourses
        {
            get
            {
                return _numberOfOngoingCourses;
            }

            set
            {
                _numberOfOngoingCourses = value;
                RaisePropertyChanged(() => NumberOfOngoingCourses);
            }
        }

        public bool IsShowMyLearningWidget
        {
            get
            {
                return _isShowMyLearningWidget;
            }

            set
            {
                _isShowMyLearningWidget = value;
                RaisePropertyChanged(() => IsShowMyLearningWidget);
            }
        }

        public bool IsShowNewsfeedWidget
        {
            get
            {
                return _isShowNewsfeedWidget;
            }

            set
            {
                _isShowNewsfeedWidget = value;
                RaisePropertyChanged(() => IsShowNewsfeedWidget);
            }
        }

        public bool IsShowRecommendForYouWidget
        {
            get
            {
                return _isShowRecommendForYouWidget;
            }

            set
            {
                _isShowRecommendForYouWidget = value;
                RaisePropertyChanged(() => IsShowRecommendForYouWidget);
            }
        }

        public bool IsShowRecommendForOrgWidget
        {
            get
            {
                return _isShowRecommendForOrgWidget;
            }

            set
            {
                _isShowRecommendForOrgWidget = value;
                RaisePropertyChanged(() => IsShowRecommendForOrgWidget);
            }
        }

        public bool IsShowBookmarkWidget
        {
            get
            {
                return _isShowBookmarkWidget;
            }

            set
            {
                _isShowBookmarkWidget = value;
                RaisePropertyChanged(() => IsShowBookmarkWidget);
            }
        }

        public bool IsVisibleSetting
        {
            get
            {
                return _isVisibleSetting;
            }

            set
            {
                _isVisibleSetting = value;
                RaisePropertyChanged(() => IsVisibleSetting);
            }
        }

        public override string PageTitle { get; } = "Home";

        public override string RoutingName => NavigationRoutes.Home;

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<ListViewModel, ItemCard>(this, "bookmarked-item-on-list");
            CourseBookmarkMessagingCenter.Unsubscribe(this);
            MessagingCenter.Unsubscribe<MyLearningDigitalContent, ItemCard>(this, "bookmarked-item-on-list");
            MessagingCenter.Unsubscribe<MyLearningDigitalContentBookmarked, ItemCard>(this, "bookmarked-item-on-list");
            MessagingCenter.Unsubscribe<WidgetSettingViewModel>(this, "home-widget-configs");
        }

        public override async Task OnComeBack()
        {
            await OutstandingTasksSection.ReloadRowIfNeeded();
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await LoadWidgetConfigs();
            await InitScreen();
        }

        private async Task InitScreen()
        {
            IsRefreshing = true;
            SettingAccessRights();

            if (!PermissionHelper.GetPermissionForHomePage())
            {
                IsRefreshing = false;
                return;
            }

            var parallelRunner = new ParallelTaskRunner(3);

            InitUserData();

            SharingSection.IsShowSharingSection = WidgetConfigs.ContainsKey(WidgetKeys.HomeSharedByOtherUsersShow) && WidgetConfigs[WidgetKeys.HomeSharedByOtherUsersShow];

            CalendarSection.IsShowCalendarSection = WidgetConfigs[WidgetKeys.HomeCalendarShow];

            var listTasks = new Func<Task>[]
            {
                InitNumberOfStatusCourse,
                CalendarSection.LoadData,
                InitNewsfeed,
                OutstandingTasksSection.LoadData,
                SharingSection.LoadData,
                InitDataForMyLearningCollection,
                InitDataForMyRecommendationsCollection,
                InitdataForRecommendationOrganizationCollection,
                InitDataForMyBookmarkCollection
            };

            try
            {
                await parallelRunner.RunAsync(listTasks);
            }
            finally
            {
                await _commonService.LearningTracking(TrackingEventType.ReachLandingPage);
                IsRefreshing = false;
            }
        }

        private async Task GoToSetting()
        {
            await NavigationService.NavigateToAsync<WidgetSettingViewModel>();
        }

        private async Task InitDataForMyLearningCollection()
        {
            if (WidgetConfigs[WidgetKeys.HomeMyLearningShow])
            {
                int myLearningTotal = 0;
                List<ItemCard> myLearningCard = new List<ItemCard>();

                var faceToFaceInprogressResult = await ExecuteBackendService(() => _learnerBackendService.GetMyCourses(
                    courseType: LearningCourseType.FaceToFace,
                    statusFilter: StatusLearning.InProgress));

                if (!faceToFaceInprogressResult.HasEmptyResult())
                {
                    myLearningTotal += faceToFaceInprogressResult.Payload.TotalCount;
                    var faceToFaceInprogressCard = await _commonService.BuildCourseCardList(faceToFaceInprogressResult.Payload.Items);
                    myLearningCard.AddRange(faceToFaceInprogressCard);
                }

                var microlearningInprogressResult = await ExecuteBackendService(() => _learnerBackendService.GetMyCourses(
                    courseType: LearningCourseType.Microlearning,
                    statusFilter: StatusLearning.InProgress));

                if (!microlearningInprogressResult.HasEmptyResult())
                {
                    myLearningTotal += microlearningInprogressResult.Payload.TotalCount;
                    var faceToFaceInprogressCard = await _commonService.BuildCourseCardList(microlearningInprogressResult.Payload.Items);
                    myLearningCard.AddRange(faceToFaceInprogressCard);
                }

                var digitalContentInprogressResult = await ExecuteBackendService(() => _learnerBackendService.GetMyLearningDigitalContent(new
                {
                    MaxResultCount = GlobalSettings.MaxResultPerPage,
                    OrderBy = "CreatedDate desc",
                    SkipCount = 0,
                    StatusFilter = StatusLearning.InProgress.ToString()
                }));

                if (!digitalContentInprogressResult.HasEmptyResult())
                {
                    myLearningTotal += digitalContentInprogressResult.Payload.TotalCount;
                }

                MyLearningTotalCount = myLearningTotal;

                MyLearningCollection = new LearnerObservableCollection<ItemCard>(myLearningCard.Skip(0).Take(10).ToList());
                RaisePropertyChanged(() => MyLearningCollection);
            }

            IsShowMyLearningWidget = WidgetConfigs[WidgetKeys.HomeMyLearningShow];
        }

        private async Task InitDataForMyRecommendationsCollection()
        {
            if (WidgetConfigs[WidgetKeys.HomeRecommendForYouShow])
            {
                var myRecommendationsCourseCardList = await _commonService.GetRecommendationsCollection(_userId, 1, total =>
                {
                    RecommendationsTotalCount = total;
                });
                MyRecommendationsCollection = new LearnerObservableCollection<ItemCard>(myRecommendationsCourseCardList);
                RaisePropertyChanged(() => MyRecommendationsCollection);
            }

            IsShowRecommendForYouWidget = WidgetConfigs[WidgetKeys.HomeRecommendForYouShow];
        }

        private async Task InitdataForRecommendationOrganizationCollection()
        {
            if (WidgetConfigs[WidgetKeys.HomeRecommendForOrgShow])
            {
                var myRecommendationOrganizationCardList = await _commonService.GetRecommendationOrganizationCollection(1, total =>
                {
                    RecommendationsOrgTotalCount = total;
                });
                MyRecommendationOrganizationCollection = new LearnerObservableCollection<ItemCard>(myRecommendationOrganizationCardList);
                RaisePropertyChanged(() => MyRecommendationOrganizationCollection);
            }

            IsShowRecommendForOrgWidget = WidgetConfigs[WidgetKeys.HomeRecommendForOrgShow];
        }

        private async Task InitDataForMyBookmarkCollection()
        {
            int bookmarkTotal = 0;

            if (WidgetConfigs[WidgetKeys.HomeBookmarkShow])
            {
                _myBookmarkCourseCardList = new List<ItemCard>();

                var courseBookmarkedCard = await _commonService.GetBookmarkedCollection(
                    bookmarkType: new string[] { BookmarkType.Course.ToString(), BookmarkType.Microlearning.ToString() },
                    totalCount: totalCourse =>
                    {
                        bookmarkTotal += totalCourse;
                    });

                if (!courseBookmarkedCard.IsNullOrEmpty())
                {
                    _myBookmarkCourseCardList.AddRange(courseBookmarkedCard);
                }

                var digitalContentBookmarkedSummary = await _commonService.GetMyDigitalContentBookmarked(totalCountAction: totalDigitalContent =>
                {
                    bookmarkTotal += totalDigitalContent;
                });

                if (!digitalContentBookmarkedSummary.IsNullOrEmpty())
                {
                    var digitalContentCard = await _commonService.CreateDigitalContentCourseCard(digitalContentBookmarkedSummary.Select(p => p.DigitalContentId).ToArray());

                    if (!digitalContentCard.IsNullOrEmpty())
                    {
                        _myBookmarkCourseCardList.AddRange(digitalContentCard);
                    }
                }

                BookmarkTotalCount = bookmarkTotal;

                _myBookmarkCourseCardList = _myBookmarkCourseCardList.OrderByDescending(p => p.BookmarkInfoChangedDate).ToList();
                MyBookmarkCollection = new LearnerObservableCollection<ItemCard>(_myBookmarkCourseCardList);
                RaisePropertyChanged(() => MyBookmarkCollection);
            }

            IsShowBookmarkWidget = WidgetConfigs[WidgetKeys.HomeBookmarkShow];
        }

        private async Task InitNewsfeed()
        {
            if (WidgetConfigs[WidgetKeys.HomeNewsfeedShow])
            {
                var newsfeed = await _commonService.GetNewsfeed(maxResultCount: 2, count: count =>
                {
                    NewsfeedTotalCount = count;
                });

                if (NewsfeedTotalCount != 0)
                {
                    NewsfeedCollection = new LearnerObservableCollection<Feed>(newsfeed);
                    RaisePropertyChanged(() => NewsfeedCollection);
                }
            }

            IsShowNewsfeedWidget = WidgetConfigs[WidgetKeys.HomeNewsfeedShow];
        }

        private void OnSeemoreNewsfeed()
        {
            if (NewsfeedTotalCount == 0)
            {
                return;
            }

            NavigationService.NavigateToAsync<ShowAllNewsfeedViewModel>();
        }

        private async Task InitNumberOfStatusCourse()
        {
            string[] statusFilters = new string[] { StatusLearning.InProgress.ToString(), StatusLearning.Completed.ToString() };

            var statusFilter = new { statusFilter = statusFilters };

            var statusApiResult = await ExecuteBackendService(() => _learnerBackendService.GetNumberOfStatusCourse(statusFilter));

            if (!statusApiResult.HasEmptyResult())
            {
                foreach (var item in statusApiResult.Payload)
                {
                    switch (item.StatusFilter)
                    {
                        case StatusLearning.InProgress:
                            NumberOfOngoingCourses = item.Total.ToString();
                            break;
                        case StatusLearning.Completed:
                            NumberOfCompleteCourses = item.Total.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void InitUserData()
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties != null)
            {
                _userId = accountProperties.User.Sub;
                Name = accountProperties.User.Name;
            }
        }

        private async Task LoadWidgetConfigs()
        {
            // Get widgets configs
            var widgetConfigsResult = await ExecuteBackendService(() => _learnerBackendService.GetUserPreference(new string[] { }));
            if (!widgetConfigsResult.HasEmptyResult() && !widgetConfigsResult.IsError)
            {
                WidgetConfigs = widgetConfigsResult.Payload.ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                WidgetConfigs = new Dictionary<string, bool>();
            }
        }

        private async Task AddItemToMyBookmarkCollection(ItemCard courseCard)
        {
            if (_myBookmarkCourseCardList == null)
            {
                return;
            }

            var existed = _myBookmarkCourseCardList.FirstOrDefault(c => c.Id == courseCard.Id);
            if (existed == null)
            {
                _myBookmarkCourseCardList.Insert(0, (ItemCard)courseCard.Clone());
                BookmarkTotalCount++;
            }
            else
            {
                _myBookmarkCourseCardList.Remove(existed);
                BookmarkTotalCount--;

                // Reload bookmark collection when no item display in bookmark collection and total bookmark count more than 0
                if (!_myBookmarkCourseCardList.Any() && BookmarkTotalCount > 0)
                {
                    await InitDataForMyBookmarkCollection();
                }

                // Update remove bookmark on order collection
                var myLearningItem = MyLearningCollection.FirstOrDefault(p => p.Id == courseCard.Id);
                if (myLearningItem != null)
                {
                    myLearningItem.BookmarkInfo = courseCard.BookmarkInfo;
                }

                var myRecommendationsItem = MyRecommendationsCollection.FirstOrDefault(p => p.Id == courseCard.Id);
                if (myRecommendationsItem != null)
                {
                    myRecommendationsItem.BookmarkInfo = courseCard.BookmarkInfo;
                }

                var myRecommendationOrganizationItem = MyRecommendationOrganizationCollection.FirstOrDefault(p => p.Id == courseCard.Id);
                if (myRecommendationOrganizationItem != null)
                {
                    myRecommendationOrganizationItem.BookmarkInfo = courseCard.BookmarkInfo;
                }
            }

            if (_myBookmarkCourseCardList.IsNullOrEmpty())
            {
                MyBookmarkCollection = new LearnerObservableCollection<ItemCard>();
            }
            else
            {
                MyBookmarkCollection = new LearnerObservableCollection<ItemCard>(_myBookmarkCourseCardList);
            }

            RaisePropertyChanged(() => MyBookmarkCollection);
        }

        private async Task OnRefresh()
        {
            await LoadWidgetConfigs();
            await InitScreen();
        }

        private void SettingAccessRights()
        {
            IsVisibleSetting = PermissionHelper.GetPermissionForHomeSetting();

            var permissionTagPage = new PermissionTabPage
            {
                IsVisibleHomePage = PermissionHelper.GetPermissionForHomePage(),
                IsVisibleCataloguePage = PermissionHelper.GetPermissionForCataloguePage(),
                IsVisibleLearningPage = PermissionHelper.GetPermissionForMyLearningPage(),
                IsVisiblePlannerPage = PermissionHelper.GetPermissionForPdPlannerPage(),
            };

            MessagingCenter.Unsubscribe<AppShell>(this, "tab-page-view");
            MessagingCenter.Send(this, "tab-page-view", permissionTagPage);
        }
    }
}
