using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Enum;
using LearnerApp.Common.Helper;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.Learner;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Controls.LearnerTabControl;
using LearnerApp.Helper;
using LearnerApp.Models;
using LearnerApp.Models.Course;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.Sharing;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.Sharing;
using LearnerApp.Views;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CourseDetailsViewModel : BasePageViewModel, ISharingContentFormDelegate
    {
        public const string AssignTabId = "AssignTab";
        public const string AboutTabId = "AboutTab";
        public const string ClassRunTabId = "ClassRunTab";
        public const string ReviewTabId = "ReviewTab";
        public const string ContentTabId = "ContentTab";

        public readonly LearnerTabControlItemViewModel ContentTab;
        public readonly LearnerTabControlItemViewModel AssignmentTab;
        public readonly LearnerTabControlItemViewModel ClassRunTab;
        public readonly LearnerTabControlItemViewModel AboutTab;
        public readonly LearnerTabControlItemViewModel ReviewTab;

        private const string LearnAgain = "LEARN AGAIN";
        private const string ViewAgain = "VIEW AGAIN";
        private const string Microlearning = "db13d0f8-d595-11e9-baec-0242ac120004";
        private const string ViewOnly = "viewOnly";

        // Backend service
        private readonly ICommonServices _commonService;
        private readonly IMetadataBackendService _metadataBackendService;
        private readonly ICourseBackendService _courseBackendService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IPDPMBackendService _pdpmBackendService;
        private readonly IUserManagementBackendService _userManagementBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;
        private readonly IIdmBackendService _idmBackendService;
        private readonly StressActionHandler _stressActionHandler = new StressActionHandler();

        // Status
        private readonly List<string> _pendingStatusList = new List<string>
        {
            RegistrationStatus.WaitlistPendingApprovalByLearner.ToString(),
            RegistrationStatus.OfferPendingApprovalByLearner.ToString()
        };

        private readonly List<string> _confirmStatusList = new List<string>
        {
            RegistrationStatus.ConfirmedByCA.ToString(),
            RegistrationStatus.OfferConfirmed.ToString()
        };

        private readonly List<string> _ignoreStatusList = new List<string>
        {
            RegistrationStatus.Rejected.ToString(),
            RegistrationStatus.RejectedByCA.ToString(),
            RegistrationStatus.OfferRejected.ToString(),
            RegistrationStatus.WaitlistRejected.ToString(),
            RegistrationStatus.Cancelled.ToString()
        };

        private readonly List<string> _inProgressStatusList = new List<string>
        {
            StatusLearning.InProgress.ToString(),
            StatusLearning.Passed.ToString(),
            StatusLearning.Failed.ToString(),
            StatusLearning.Completed.ToString()
        };

        private string _courseName;
        private string _myCourseId;
        private string _courseDescription;
        private string _duration;
        private string _courseCode;
        private string _lectureTitle;
        private double _rating;
        private bool _isVisibleUnenroll;
        private bool _isVisibleShowMoreReviews;
        private bool _isVisibleReviewStack;
        private bool _isVisibleAboutStack;
        private bool _isCourseCompleted;
        private bool _isVisibleAboutInformationStack;
        private bool _isVisibleParticipate;
        private bool _isVisibleUpComingSession;
        private string _learningStatus;
        private UserReview _ownReview;
        private CourseExtendedInformation _courseExtendedInformation;
        private List<string> _tagList;
        private List<TableOfContent> _tableOfContentData;
        private List<MyLecturesInfo> _myTableOfContentData;
        private LearningContentTransfer _learningContentTransferData;
        private EditReview _editReview;
        private List<TableOfContent> _lectures;
        private LearningOpportunityClassRunCardTransfer _learningOpportunityClassRunCardTransfer;
        private LearningOpportunityCardTransfer _learningOpportunityCardTransfer;
        private LearningOpportunityInformationCardTransfer _learningOpportunityInformationCardTransfer;
        private string _title;
        private bool _isRefreshing = true;
        private List<MetadataTag> _metadataTags = new List<MetadataTag>();

        // Registration flow
        private string _userId;
        private string _resultId;
        private string _approvingOfficerId;
        private string _alternativeApprovingOfficerId;
        private bool _isMicroLearningType;
        private bool _isScrolling;
        private MyClassRun _myClassRun = new MyClassRun();
        private MyCourseSummary _myCourseSummary = new MyCourseSummary();
        private MyCourseInfo _myCourseInfo = new MyCourseInfo();
        private List<MyClassRun> _myClassRuns = new List<MyClassRun>();
        private MyCourseStatus _myCourseStatus = new MyCourseStatus();

        private string _myClassRunPendingId;
        private string _myClassRunName;
        private string _pendingPopupTitle;
        private bool _isAddToPlan;
        private bool _isVisibleLearningStatus = true;

        // Permission
        private bool _isVisibleLike = true;
        private bool _isVisibleMore = true;
        private bool _isVisibleBookmark = true;
        private bool _isVisibleCopy = true;
        private bool _isVisibleShare = true;

        // Post course evaluation
        private bool _isVisiblePostCourse;
        private bool _isEnablePostCourse;

        // Upcoming session
        private UpComingTransferData _upComingSession;

        // Assignment
        private ParticipantAssignmentTracksRequest _participantAssignmentTracksRequest;
        private bool _isVisibleAssignment = true;
        private List<AssignmentDetail> _assignmentDetails;
        private LearnerObservableCollection<LearnerTabControlItemViewModel> _tabItemSource = new LearnerObservableCollection<LearnerTabControlItemViewModel>();

        // Content
        private bool _isTableOfContentEmpty;
        private string _courseId;
        private BookmarkType _cardType;
        private bool _isLoadingCourseDetailData;

        public CourseDetailsViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _pdpmBackendService = CreateRestClientFor<IPDPMBackendService>(GlobalSettings.BackendServicePDPM);
            _userManagementBackendService = CreateRestClientFor<IUserManagementBackendService>(GlobalSettings.BackendServiceUserManagement);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
            _metadataBackendService = CreateRestClientFor<IMetadataBackendService>(GlobalSettings.BackendServiceTagging);
            _commonService = DependencyService.Resolve<ICommonServices>();
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
            _tagList = new List<string>();

            CachingMode = PageCachingMode.None;

            AboutTab = new LearnerTabControlItemViewModel(AboutTabId, "ABOUT")
            {
                IsSelected = true,
                IsVisible = true
            };

            ContentTab = new LearnerTabControlItemViewModel(ContentTabId, "CONTENT")
            {
                IsVisible = true
            };

            AssignmentTab = new LearnerTabControlItemViewModel(AssignTabId, "ASSIGNMENTS");
            ClassRunTab = new LearnerTabControlItemViewModel(ClassRunTabId, "CLASS RUNS");
            ReviewTab = new LearnerTabControlItemViewModel(ReviewTabId, "REVIEWS");

            TabItemSource = new LearnerObservableCollection<LearnerTabControlItemViewModel>()
            {
                AboutTab,
                ClassRunTab,
                ContentTab,
                AssignmentTab,
                ReviewTab,
            };

            MessagingCenter.Subscribe<LearningContentViewModel>(this, "on-closed-quiz", async (sender) =>
            {
                // Update content list when closed LearningContentView
                await Task.Delay(1000);
                await InitScreen();
            });

            MessagingCenter.Subscribe<LearningOpportunityFeedbackCard>(this, "on-submit-feedback", async (sender) =>
            {
                // Update content list when closed LearningContentView
                await InitScreen();
            });

            MessagingCenter.Subscribe<CheckInViewModel>(this, "on-back-to-course-details", async (sender) =>
            {
                await InitScreen(false);
            });

            MessagingCenter.Subscribe<CourseDetailsView>(this, "on-reload-course-details", async (sender) =>
            {
                await Task.Delay(1000);
                await InitScreen(false);
            });

            MessagingCenter.Subscribe<PostCourseViewModel>(this, "on-back-to-course-details-from-post-course", async (sender) =>
            {
                await Task.Delay(1000);
                await InitScreen(false);
            });

            MessagingCenter.Subscribe<CannotParticipateViewModel>(this, "submit-absence-reason-success", async (sender) =>
            {
                IsVisibleParticipate = false;

                await InitScreen(false);
            });

            MessagingCenter.Subscribe<LearningOpportunityClassRunCardViewModel, UpComingTransferData>(this, "upcoming-session", (sender, upComingTransferData) =>
            {
                UpComingSession = upComingTransferData;
                IsVisibleUpComingSession = true;
            });

            MessagingCenter.Subscribe<LearningOpportunityClassRunCardViewModel, string>(this, "reload-toc", async (sender, classRunId) =>
            {
                using (DialogService.DisplayLoadingIndicator())
                {
                    await ReloadTableOfContent(classRunId);
                }
            });
        }

        public bool IsVisibleParticipate
        {
            get
            {
                return _isVisibleParticipate;
            }

            set
            {
                _isVisibleParticipate = value;

                RaisePropertyChanged(() => IsVisibleParticipate);
            }
        }

        public bool IsVisibleUpComingSession
        {
            get
            {
                return _isVisibleUpComingSession;
            }

            set
            {
                _isVisibleUpComingSession = value;

                RaisePropertyChanged(() => IsVisibleUpComingSession);
            }
        }

        public string CourseName
        {
            get
            {
                return _courseName;
            }

            set
            {
                _courseName = value;

                RaisePropertyChanged(() => CourseName);
            }
        }

        public string CourseDescription
        {
            get
            {
                return _courseDescription;
            }

            set
            {
                _courseDescription = value;

                RaisePropertyChanged(() => CourseDescription);

                IsVisibleAboutStack = true;
                IsVisibleAboutInformationStack = false;
            }
        }

        public string Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;

                RaisePropertyChanged(() => Duration);
            }
        }

        public string CourseCode
        {
            get
            {
                return _courseCode;
            }

            set
            {
                _courseCode = value;
                RaisePropertyChanged(() => CourseCode);
            }
        }

        public double Rating
        {
            get
            {
                return _rating;
            }

            set
            {
                _rating = value;
                RaisePropertyChanged(() => Rating);
            }
        }

        public string LectureTitle
        {
            get
            {
                return _lectureTitle;
            }

            set
            {
                _lectureTitle = value;
                RaisePropertyChanged(() => LectureTitle);
            }
        }

        public string LearningStatus
        {
            get
            {
                return _learningStatus;
            }

            set
            {
                _learningStatus = value;
                RaisePropertyChanged(() => LearningStatus);
            }
        }

        public List<string> TagList
        {
            get
            {
                return _tagList;
            }

            set
            {
                _tagList = value;
                RaisePropertyChanged(() => TagList);
            }
        }

        public List<TableOfContent> TableOfContentData
        {
            get
            {
                return _tableOfContentData;
            }

            set
            {
                _tableOfContentData = value;
                RaisePropertyChanged(() => TableOfContentData);
            }
        }

        public List<MyLecturesInfo> MyTableOfContentData
        {
            get
            {
                return _myTableOfContentData;
            }

            set
            {
                _myTableOfContentData = value;
                RaisePropertyChanged(() => MyTableOfContentData);
            }
        }

        public LearningContentTransfer LearningContentTransferData
        {
            get
            {
                return _learningContentTransferData;
            }

            set
            {
                _learningContentTransferData = value;
                RaisePropertyChanged(() => LearningContentTransferData);
            }
        }

        public EditReview EditReview
        {
            get
            {
                return _editReview;
            }

            set
            {
                _editReview = value;
                RaisePropertyChanged(() => EditReview);
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public LearningOpportunityClassRunCardTransfer LearningOpportunityClassRunCardTransfer
        {
            get
            {
                return _learningOpportunityClassRunCardTransfer;
            }

            set
            {
                _learningOpportunityClassRunCardTransfer = value;
                RaisePropertyChanged(() => LearningOpportunityClassRunCardTransfer);
            }
        }

        public LearningOpportunityCardTransfer LearningOpportunityCardTransfer
        {
            get
            {
                return _learningOpportunityCardTransfer;
            }

            set
            {
                _learningOpportunityCardTransfer = value;
                RaisePropertyChanged(() => LearningOpportunityCardTransfer);
            }
        }

        public LearningOpportunityInformationCardTransfer LearningOpportunityInformationCardTransfer
        {
            get
            {
                return _learningOpportunityInformationCardTransfer;
            }

            set
            {
                _learningOpportunityInformationCardTransfer = value;
                RaisePropertyChanged(() => LearningOpportunityInformationCardTransfer);
            }
        }

        public bool IsVisiblePostCourse
        {
            get
            {
                return _isVisiblePostCourse;
            }

            set
            {
                _isVisiblePostCourse = value;
                RaisePropertyChanged(() => IsVisiblePostCourse);
            }
        }

        public bool IsEnablePostCourse
        {
            get
            {
                return _isEnablePostCourse;
            }

            set
            {
                _isEnablePostCourse = value;
                RaisePropertyChanged(() => IsEnablePostCourse);
            }
        }

        public bool IsVisibleAssignment
        {
            get
            {
                return _isVisibleAssignment;
            }

            set
            {
                _isVisibleAssignment = value;
                AssignmentTab.IsVisible = !value;
                RaisePropertyChanged(() => IsVisibleAssignment);
            }
        }

        public List<AssignmentDetail> AssignmentDetails
        {
            get
            {
                return _assignmentDetails;
            }

            set
            {
                _assignmentDetails = value;
                RaisePropertyChanged(() => AssignmentDetails);
            }
        }

        public bool IsVisibleLearningStatus
        {
            get
            {
                return _isVisibleLearningStatus;
            }

            set
            {
                _isVisibleLearningStatus = value;
                RaisePropertyChanged(() => IsVisibleLearningStatus);
            }
        }

        public bool IsVisibleLike
        {
            get
            {
                return _isVisibleLike;
            }

            set
            {
                _isVisibleLike = value;
                RaisePropertyChanged(() => IsVisibleLike);
            }
        }

        public bool IsVisibleMore
        {
            get
            {
                return _isVisibleMore;
            }

            set
            {
                _isVisibleMore = value;
                RaisePropertyChanged(() => IsVisibleMore);
            }
        }

        public bool IsCourseCompleted => _isCourseCompleted;

        public bool IsTableOfContentEmpty => _isTableOfContentEmpty;

        public ICommand MoreCommand => new Command(async () => await OnMoreTapped());

        public ICommand EnrollCommand => new Command(async () => await OnEnrollTapped());

        /* OP-8577 Must hide Stop Learning' link
        public ICommand UnenrollCommand => new Command(async () => await OnUnenrollTapped());*/

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand DeclineClassRunCommand => new Command(async () => await DeclineClassRun());

        public ICommand AcceptClassRunCommand => new Command(async () => await AcceptClassRun());

        public ICommand PostCourseCommand => new Command(async () => await OpenPostCourse());

        public ICommand EditUserCommand => new Command((review) => OnEditUserComment(review as UserReview));

        public ICommand OpenCourseCommunityCommand => new Command(async () => await OpenCourseCommunity());

        public ICommand LikeCommand => new Command(async () => await LikeTapped());

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseDetails;

        public bool IsVisibleReviewStack
        {
            get
            {
                return _isVisibleReviewStack;
            }

            set
            {
                _isVisibleReviewStack = value;
                ReviewTab.IsVisible = value;
                RaisePropertyChanged(() => IsVisibleReviewStack);
            }
        }

        public bool IsVisibleAboutInformationStack
        {
            get
            {
                return _isVisibleAboutInformationStack;
            }

            set
            {
                _isVisibleAboutInformationStack = value;
                RaisePropertyChanged(() => IsVisibleAboutInformationStack);
            }
        }

        public bool IsVisibleUnenroll
        {
            get
            {
                return _isVisibleUnenroll;
            }

            set
            {
                _isVisibleUnenroll = value;

                RaisePropertyChanged(() => IsVisibleUnenroll);
            }
        }

        public bool IsVisibleShowMoreReviews
        {
            get
            {
                return _isVisibleShowMoreReviews;
            }

            set
            {
                _isVisibleShowMoreReviews = value;
                RaisePropertyChanged(() => IsVisibleShowMoreReviews);
            }
        }

        public bool IsVisibleAboutStack
        {
            get
            {
                return _isVisibleAboutStack;
            }

            set
            {
                _isVisibleAboutStack = value;
                RaisePropertyChanged(() => IsVisibleAboutStack);
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

        public bool IsLoadingCourseDetailData
        {
            get
            {
                return _isLoadingCourseDetailData;
            }

            set
            {
                _isLoadingCourseDetailData = value;
                RaisePropertyChanged(() => IsLoadingCourseDetailData);
            }
        }

        public bool IsMicroLearningType
        {
            get
            {
                return _isMicroLearningType;
            }

            set
            {
                _isMicroLearningType = value;
                ClassRunTab.IsVisible = !value;
                RaisePropertyChanged(() => IsMicroLearningType);
            }
        }

        public MyCourseStatus MyCourseStatus
        {
            get
            {
                return _myCourseStatus;
            }

            set
            {
                _myCourseStatus = value;
                RaisePropertyChanged(() => MyCourseStatus);
            }
        }

        public string MyClassRunName
        {
            get
            {
                return _myClassRunName;
            }

            set
            {
                _myClassRunName = value;
                RaisePropertyChanged(() => MyClassRunName);
            }
        }

        public string PendingPopupTitle
        {
            get
            {
                return _pendingPopupTitle;
            }

            set
            {
                _pendingPopupTitle = value;
                RaisePropertyChanged(() => PendingPopupTitle);
            }
        }

        public ParticipantAssignmentTracksRequest ParticipantAssignmentTracksRequest
        {
            get
            {
                return _participantAssignmentTracksRequest;
            }

            set
            {
                _participantAssignmentTracksRequest = value;
                RaisePropertyChanged(() => ParticipantAssignmentTracksRequest);
            }
        }

        public UpComingTransferData UpComingSession
        {
            get
            {
                return _upComingSession;
            }

            set
            {
                _upComingSession = value;
                RaisePropertyChanged(() => UpComingSession);
            }
        }

        public ClassRun ClassRunInProgress { get; set; }

        public LearnerObservableCollection<LearnerTabControlItemViewModel> TabItemSource
        {
            get => _tabItemSource;
            set
            {
                _tabItemSource = value;
                RaisePropertyChanged(() => TabItemSource);
            }
        }

        public bool IsScrolling
        {
            get
            {
                return _isScrolling;
            }

            set
            {
                _isScrolling = value;
                RaisePropertyChanged(() => IsScrolling);
            }
        }

        public string ThumbnailUrl => _courseExtendedInformation?.ThumbnailUrl;

        public bool IsRefreshingForRefreshView
        {
            get
            {
                return false;
            }

            set
            {
                if (!_isRefreshing)
                {
                    RaisePropertyChanged(() => IsRefreshingForRefreshView);
                }
            }
        }

        public static NavigationParameters GetNavigationParameters(string courseId, BookmarkType type, bool startPlayer = false)
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.SetParameter("course-id", courseId);
            navigationParameter.SetParameter("start-player", startPlayer);
            navigationParameter.SetParameter("bookmark-type", type);
            return navigationParameter;
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<LearningContentViewModel>(this, "on-closed-quiz");
            MessagingCenter.Unsubscribe<LearningOpportunityFeedbackCard>(this, "on-submit-feedback");
            MessagingCenter.Unsubscribe<CheckInViewModel>(this, "on-back-to-course-details");
            MessagingCenter.Unsubscribe<CourseDetailsView>(this, "on-reload-course-details");
            MessagingCenter.Unsubscribe<PostCourseViewModel>(this, "on-back-to-course-details-from-post-course");
            MessagingCenter.Unsubscribe<CannotParticipateViewModel>(this, "submit-absence-reason-success");
            MessagingCenter.Unsubscribe<LearningOpportunityClassRunCardViewModel, UpComingTransferData>(this, "upcoming-session");
            MessagingCenter.Unsubscribe<LearningOpportunityClassRunCardViewModel, string>(this, "reload-toc");
        }

        public async Task<bool> AddShareUser(UserInformation userInformation)
        {
            var result = await ExecuteBackendService(
                () => _learnerBackendService.ShareContent(
                    new ShareContentArgumentsPayload()
                    {
                        ItemId = _courseId,
                        ItemType = IsMicroLearningType ? BookmarkType.Microlearning.ToString() : BookmarkType.Course.ToString(),
                        SharedUsers = new string[] { userInformation.UserCxId }
                    }));

            return result;
        }

        public void ClearTabSelected()
        {
            foreach (var item in TabItemSource)
            {
                item.IsSelected = false;
            }
        }

        public async Task GetCourseDetailDataForContentView()
        {
            IsLoadingCourseDetailData = true;
            try
            {
                // Prerequisite Courses
                var prerequisiteCourses = new List<PrerequisiteCourse>();
                var metadataSearching = Application.Current.Properties.GetMetadataSearching();
                if (metadataSearching.IsNullOrEmpty())
                {
                    var preRequisite = new { SearchType = 4, MaxResultCount = 100000 };
                    var searching =
                        await ExecuteBackendService(() => _courseBackendService.GetPreRequisites(preRequisite));
                    if (!searching.HasEmptyResult())
                    {
                        Application.Current.Properties.AddMetadataSearching(searching.Payload.Items);
                    }
                }
                else
                {
                    prerequisiteCourses = metadataSearching;
                }

                // Department Courses
                var departments = Application.Current.Properties.GetMetadataDepartment()
                                  ?? new List<Department>();

                // Collaborative ContentCreator Ids
                var collaborativeContentCreatorIds = new List<UserInformation>();

                if (_courseExtendedInformation != null &&
                    !_courseExtendedInformation.CollaborativeContentCreatorIds.IsNullOrEmpty())
                {
                    var userCxIds = new { UserCxIds = _courseExtendedInformation.CollaborativeContentCreatorIds };

                    var userResponse =
                        await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(userCxIds));
                    if (!userResponse.HasEmptyResult())
                    {
                        collaborativeContentCreatorIds = userResponse.Payload;
                    }
                }

                SetLearningOpportunityCardTransfer(
                    _metadataTags,
                    _courseExtendedInformation,
                    prerequisiteCourses,
                    departments,
                    collaborativeContentCreatorIds);
            }
            finally
            {
                IsLoadingCourseDetailData = false;
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            _courseId = navigationParameters.GetParameter<string>("course-id");
            _cardType = navigationParameters.GetParameter<BookmarkType>("card-type");

            await InitScreen();

            // Page visited user tracking
            var payload = new TrackingDigitalContent
            {
                ItemId = _courseId,
                ItemType = "course",
                TrackingAction = "view"
            };
            await ExecuteBackendService(() => _commonService.LearningTracking(TrackingEventType.ReachPDOPage, payload));

            if (navigationParameters.GetParameter<bool>("start-player"))
            {
                EnrollCommand?.Execute(null);
            }
        }

        private async Task ReloadTableOfContent(string classRunId)
        {
            await GetTableOfContent(classRunId);
        }

        private async Task PostEnroll(List<string> lectureIds)
        {
            await ExecuteBackendService(() => _learnerBackendService.CreateCourseEnrollment(new { courseId = _courseId, lectureIds }));
            await ReloadCourseAndStatus();
        }

        private async Task RePostEnroll(List<string> lectureIds)
        {
            var reEnrollCourse = new ReEnrollCourse
            {
                CourseId = _courseId,
                LectureIds = lectureIds,
                CourseType = LearningCourseType.Microlearning
            };
            await ExecuteBackendService(() => _learnerBackendService.CreateCourseReEnrollment(reEnrollCourse));
            await ReloadCourseAndStatus();
        }

        private async Task ReloadCourseAndStatus()
        {
            await GetCourseExtraInformation();
            LearningStatus = CourseLearningStatus.CONTINUE.ToString();
            MyCourseStatus.Status = StatusLearning.InProgress.ToString();
        }

        /* OP-8577 Must hide Stop Learning' link
        private async Task OnUnenrollTapped()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                await ExecuteBackendService(() => _learnerBackendService.DeleteCourseEnrollment(_myCourseId));

                LearningStatus = CourseLearningStatus.START.ToString();
            }
        }*/

        private async Task OnEnrollTapped()
        {
            if (LearningStatus == CourseLearningStatus.ADDTOPDPLAN.ToString())
            {
                _resultId = await PDPlanMethod.AddCourseToPDPlan(_courseId, _userId, (pdo) => ExecuteBackendService(() => _pdpmBackendService.AddCourseToPDPlan(pdo)));

                if (!string.IsNullOrEmpty(_resultId))
                {
                    await DialogService.ShowAlertAsync(TextsResource.COURSE_ADDED_PDPLAN, "OK", isVisibleIcon: false);
                    LearningStatus = CourseLearningStatus.REMOVEFROMPDPLAN.ToString();
                }
            }
            else if (LearningStatus == CourseLearningStatus.REMOVEFROMPDPLAN.ToString())
            {
                await PDPlanMethod.DeactivateCourseInPDPlan(_resultId, (pdoRemove) => ExecuteBackendService(() => _pdpmBackendService.DeactivateCourseInPDPlan(pdoRemove)));
                await DialogService.ShowAlertAsync(TextsResource.COURSE_REMOVED_PDPLAN, "OK", isVisibleIcon: false);
                LearningStatus = CourseLearningStatus.ADDTOPDPLAN.ToString();
            }
            else
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    await GoToLearningContent();
                    IsBusy = false;
                }
            }
        }

        private async Task GoToLearningContent()
        {
            if (!IsMicroLearningType)
            {
                if (LearningStatus == CourseLearningStatus.START.ToString())
                {
                    var classRunResponse = await ExecuteBackendService(() => _courseBackendService.GetClassRunById(_myClassRun.ClassRunId));
                    var classRun = new ClassRun();

                    if (!classRunResponse.HasEmptyResult())
                    {
                        classRun = classRunResponse.Payload;
                    }

                    var classRunIds = new[] { _myClassRun.ClassRunId };
                    var sessionResponse = await ExecuteBackendService(() => _courseBackendService.GetSessionsByClassRunIds(classRunIds));

                    if (!sessionResponse.HasEmptyResult())
                    {
                        if (classRun != null)
                        {
                            classRun.Sessions = sessionResponse.Payload;
                        }
                    }

                    bool isEmptySession = classRun != null && classRun.Sessions.IsNullOrEmpty();

                    // If the class has no content
                    if (_lectures.IsNullOrEmpty())
                    {
                        await PostEnroll(null);

                        CrossToastPopUp.Current.ShowToastWarning(isEmptySession ? TextsResource.NO_CONTENT_SESSION : TextsResource.NO_CONTENT, ToastLength.Long);

                        await InitScreen(false);
                        return;
                    }

                    // If the class has no session but have lecture
                    if (isEmptySession)
                    {
                        CrossToastPopUp.Current.ShowToastWarning(TextsResource.NO_SESSION, ToastLength.Long);
                    }
                }
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                bool hasContentChanged;

                var lectureIds = _lectures.Select(p => p.Id).ToList();

                if (LearningStatus == CourseLearningStatus.START.ToString())
                {
                    var payload = new TrackingLecture
                    {
                        LectureId = lectureIds.First(),
                        CourseId = _courseId
                    };

                    await _commonService.LearningTracking(TrackingEventType.StartLecture, payload);
                    await PostEnroll(lectureIds);
                    await ShowLearningContentView(false, false);
                }
                else if (LearningStatus == LearnAgain && _myCourseInfo != null && _myCourseInfo.HasContentChanged)
                {
                    var payload = new TrackingViewOrLearnAgain
                    {
                        VisitMode = ViewOnly,
                        CourseId = _courseId
                    };

                    await _commonService.LearningTracking(TrackingEventType.RevisitMLU, payload);
                    await RePostEnroll(lectureIds);
                    await ShowLearningContentView(true, false);
                }
                else if (LearningStatus == CourseLearningStatus.CONTINUE.ToString())
                {
                    if (IsMicroLearningType)
                    {
                        // Check content has changed in micro-learning type
                        // In case not refresh view
                        var myCourseSummaryInfo = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(_courseId));

                        if (myCourseSummaryInfo.HasEmptyResult())
                        {
                            return;
                        }

                        var myCourseSummary = myCourseSummaryInfo.Payload;
                        _myCourseInfo = myCourseSummary?.MyCourseInfo ?? new MyCourseInfo();

                        hasContentChanged = true;

                        if (_myCourseInfo != null && _myCourseInfo.HasContentChanged)
                        {
                            await GetTableOfContent();
                            lectureIds = _lectures.Select(p => p.Id).ToList();

                            await this.DialogService.ShowAlertAsync(TextsResource.UPDATE_CONTENT, "OK", async (confirmed) =>
                            {
                                if (!confirmed)
                                {
                                    return;
                                }

                                await RePostEnroll(lectureIds);
                                await ShowLearningContentView(hasContentChanged, false);
                            });
                        }
                        else
                        {
                            await ShowLearningContentView(true, false);
                        }
                    }
                    else
                    {
                        await ShowLearningContentView(false, false);
                    }
                }
                else if (LearningStatus == ViewAgain)
                {
                    var payload = new TrackingViewOrLearnAgain
                    {
                        VisitMode = ViewOnly,
                        CourseId = _courseId
                    };

                    await _commonService.LearningTracking(TrackingEventType.RevisitMLU, payload);
                    await ShowLearningContentView(false, true);
                }
                else
                {
                    await ShowLearningContentView(false, false);
                }
            }
        }

        private async Task ShowLearningContentView(bool hasContentChanged, bool isViewAgain)
        {
            // lectureIndex = -1 that mean user click enroll button
            LearningContentTransfer learningContentTransfer = new LearningContentTransfer
            {
                Lectures = _lectures,
                CourseId = _courseId,
                MyCourseId = _myCourseId,
                IsCourseCompleted = _isCourseCompleted,
                ThumbnailUrl = _courseExtendedInformation.ThumbnailUrl,
                LectureIndex = -1,
                CourseName = CourseName,
                OwnReview = _ownReview,
                HasContentChanged = hasContentChanged,
                ClassRunId = _myClassRun?.ClassRunId,
            };

            string eCertificatePrerequisite = string.IsNullOrEmpty(_courseExtendedInformation.ECertificatePrerequisite) ? string.Empty : _courseExtendedInformation.ECertificatePrerequisite;
            await NavigationService.NavigateToAsync<LearningContentViewModel>(
                LearningContentViewModel.GetNavigationParameters(
                    learningContentTransfer,
                    _isMicroLearningType,
                    eCertificatePrerequisite,
                    isViewAgain));
        }

        private void SetCourseExtendedInformation(CourseExtendedInformation data)
        {
            _courseExtendedInformation = data;
            RaisePropertyChanged(() => ThumbnailUrl);
        }

        private async Task GetCourseBasicInformation()
        {
            var courseResponse = await ExecuteBackendService(() => _courseBackendService.GetCourseByCourseId(_courseId));
            if (courseResponse.HasEmptyResult())
            {
                return;
            }

            SetCourseExtendedInformation(courseResponse.Payload);
            var metadataTagging = await GetMetadataTagging();

            // Check MLU type by PdActivityType
            IsMicroLearningType = _courseExtendedInformation.PdActivityType.Equals(Microlearning);

            var tags = new List<string>();

            // PDO Opportunity
            var pdType = metadataTagging.FirstOrDefault(p => p.TagId == _courseExtendedInformation.PdActivityType);

            if (pdType != null)
            {
                tags.Add(pdType.DisplayText);
            }

            // Mode of Learning
            var learningMode = metadataTagging.FirstOrDefault(p => p.TagId == _courseExtendedInformation.LearningMode);

            if (learningMode != null)
            {
                tags.Add(learningMode.DisplayText);
            }

            // PD Subject / Theme
            var pdSubjectTheme = metadataTagging.FirstOrDefault(p => p.TagId == _courseExtendedInformation.PdAreaThemeId);

            if (pdSubjectTheme != null)
            {
                tags.Add(pdSubjectTheme.DisplayText);
            }

            TagList = tags.ToList();

            if (!IsMicroLearningType)
            {
                var registrationMethod = _courseExtendedInformation.RegistrationMethod ?? RegistrationMethod.None;

                if (registrationMethod == RegistrationMethod.Private)
                {
                    IsVisibleLearningStatus = false;
                }
            }

            CourseName = _courseExtendedInformation.CourseName;
            CourseDescription = _courseExtendedInformation.Description;
            CourseCode = _courseExtendedInformation.CourseCode ?? GlobalSettings.NotAvailable;
            Duration = _courseExtendedInformation.DurationMinutes.ToString();

            if (!metadataTagging.IsNullOrEmpty())
            {
                _metadataTags = metadataTagging;
            }

            var trackingInfoResponse = await ExecuteBackendService(() => _learnerBackendService.GetTrackingInfo(new
            {
                itemId = _courseId,
                itemType = _isMicroLearningType ? "Microlearning" : "Course"
            }));

            if (!trackingInfoResponse.HasEmptyResult())
            {
                _courseExtendedInformation.TotalView = trackingInfoResponse.Payload.TotalView;
                _courseExtendedInformation.TotalShare = trackingInfoResponse.Payload.TotalShare;
                _courseExtendedInformation.IsLike = trackingInfoResponse.Payload.IsLike;
                _courseExtendedInformation.TotalLike = trackingInfoResponse.Payload.TotalLike;
            }

            LearningOpportunityInformationCardTransfer = new LearningOpportunityInformationCardTransfer
            {
                CourseExtendedInformation = _courseExtendedInformation,
                MetadataTags = _metadataTags
            };
        }

        private async Task GetCourseExtraInformation()
        {
            // Visible upcoming session when init screen
            IsVisibleUpComingSession = false;

            var myCourseSummaryInfo = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(_courseId));

            if (myCourseSummaryInfo.HasEmptyResult())
            {
                Rating = 0;
                await SetStatusOfCourseNotYetStudied();
                return;
            }

            _myCourseSummary = myCourseSummaryInfo.Payload;

            if (_myCourseSummary.CompletedTimes == _courseExtendedInformation.MaxReLearningTimes)
            {
                if (_myCourseInfo?.Status != StatusLearning.Completed.ToString() && !string.IsNullOrEmpty(_myCourseInfo?.Status))
                {
                    IsVisibleLearningStatus = false;
                }
            }

            _myClassRun = _myCourseSummary.MyClassRuns.IsNullOrEmpty() ? new MyClassRun() : _myCourseSummary.MyClassRuns[0];
            _myClassRuns = _myCourseSummary.MyClassRuns.IsNullOrEmpty() ? new List<MyClassRun>() : _myClassRuns = _myCourseSummary.MyClassRuns;
            _myCourseInfo = _myCourseSummary.MyCourseInfo;

            Rating = _myCourseSummary.Rating;

            if (_myCourseInfo == null)
            {
                await SetStatusOfCourseNotYetStudied();
                return;
            }

            _isCourseCompleted = _myCourseInfo.Status == StatusLearning.Completed.ToString();
            _myCourseId = _myCourseInfo.Id;

            bool isNominated = _myClassRun.RegistrationType == RegistrationType.Nominated || _myClassRun.RegistrationType == RegistrationType.AddedByCA;

            if (_myClassRun?.ClassRunId != null)
            {
                var classRunInProgress = await ExecuteBackendService(() => _courseBackendService.GetClassRunById(_myClassRun?.ClassRunId));

                if (!classRunInProgress.HasEmptyResult())
                {
                    ClassRunInProgress = classRunInProgress.Payload;
                }
            }

            // Check start date and in progress course to visible learning status
            if (_myClassRun.LearningStatus != StatusLearning.NotStarted)
            {
                var localNow = DateTime.Now.ToUniversalTime().Date;
                if (ClassRunInProgress?.GetStartDateTime().Date > localNow)
                {
                    _isVisibleLearningStatus = false;
                }

                // Allow learner to view the content of the course after they have completed
                if (_myCourseInfo.Status == StatusLearning.Completed.ToString())
                {
                    if (localNow > ClassRunInProgress?.GetEndDateTime().Date)
                    {
                        _isVisibleLearningStatus = false;
                    }
                    else
                    {
                        LearningStatus = ViewAgain;
                    }
                }
            }

            MyCourseStatus = new MyCourseStatus
            {
                Status = _myCourseInfo?.Status == StatusLearning.Failed.ToString() ? StatusLearning.Incomplete.ToString() : _myCourseInfo?.Status,
                MyRegistrationStatus = _myCourseInfo?.MyRegistrationStatus,
                MyWithdrawalStatus = _myCourseInfo?.MyWithdrawalStatus,
                MyCourseDisplayStatus = _myCourseInfo?.DisplayStatus.ToString(),
                IsNominated = isNominated,
                IsAddToPlan = _isAddToPlan,
                IsMicroLearningType = _isMicroLearningType,
                ClassRunStatus = ClassRunInProgress?.Status ?? ClassRunStatus.None,
                IsVisibleLearningStatus = _myCourseInfo?.Status != StatusLearning.Failed.ToString() && _isVisibleLearningStatus,
                LearningStatus = _learningStatus,
                IsCourseCompleted = _isCourseCompleted,
                IsTableOfContentEmpty = _isTableOfContentEmpty
            };

            // Registration/Nomination unsuccessfully
            if (_myCourseSummary?.ExpiredMyClassRun != null)
            {
                string typeOfRegistration = _myCourseSummary.ExpiredMyClassRun.RegistrationType == RegistrationType.Manual ? "Registration" : "Nomination";

                MyCourseStatus = new MyCourseStatus
                {
                    Status = MyCourseStatus.Status,
                    MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                    MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                    MyCourseDisplayStatus = $"{typeOfRegistration} Unsuccessful",
                    IsNominated = MyCourseStatus.IsNominated,
                    IsAddToPlan = MyCourseStatus.IsAddToPlan,
                    IsMicroLearningType = _isMicroLearningType,
                    ClassRunStatus = MyCourseStatus.ClassRunStatus,
                    IsVisibleLearningStatus = _isVisibleLearningStatus,
                    LearningStatus = _learningStatus,
                    IsCourseCompleted = _isCourseCompleted,
                    IsTableOfContentEmpty = _isTableOfContentEmpty
                };
            }

            // Get list of assignment
            if (_inProgressStatusList.Contains(MyCourseStatus.Status) && !IsMicroLearningType)
            {
                IsVisibleAssignment = false;

                ParticipantAssignmentTracksRequest = new ParticipantAssignmentTracksRequest
                {
                    ClassRunId = _myClassRun.ClassRunId,
                    RegistrationId = _myClassRun.RegistrationId,
                    CourseId = _courseId,
                    ForCurrentUser = true,
                    IncludeQuizAssignmentFormAnswer = false
                };

                await GetMyAssignments(ParticipantAssignmentTracksRequest);
            }
            else
            {
                IsVisibleAssignment = true;
            }

            if (_myCourseInfo?.Status == StatusLearning.InProgress.ToString()
                || _myCourseInfo?.Status == StatusLearning.Passed.ToString())
            {
                LearningStatus = CourseLearningStatus.CONTINUE.ToString();
                return;
            }

            if (_myCourseInfo?.PostCourseEvaluationFormCompleted != null
                && _myCourseInfo.PostCourseEvaluationFormCompleted.Value
                && _myCourseInfo?.Status == StatusLearning.Failed.ToString())
            {
                await SetLearningStatusByCurrentYear();
                return;
            }

            if (_myCourseInfo?.PostCourseEvaluationFormCompleted == null && _myCourseInfo?.Status == StatusLearning.Failed.ToString())
            {
                await SetLearningStatusByCurrentYear();
                return;
            }

            if (_myCourseInfo?.PostCourseEvaluationFormCompleted != null
                     && !_myCourseInfo.PostCourseEvaluationFormCompleted.Value
                     && _myCourseInfo?.Status == StatusLearning.Failed.ToString())
            {
                await SetLearningStatusByCurrentYear();
            }

            // Set status by current year
            if (!IsMicroLearningType)
            {
                if (!_inProgressStatusList.Contains(_myCourseInfo.Status))
                {
                    await SetLearningStatusByCurrentYear();
                }
            }

            // Set learning status follow type of micro-learning and status of class run
            if (IsMicroLearningType)
            {
                if (_myCourseInfo.HasContentChanged)
                {
                    LearningStatus = _isCourseCompleted ? LearnAgain : CourseLearningStatus.CONTINUE.ToString();
                }
                else
                {
                    LearningStatus = _isCourseCompleted ? ViewAgain : CourseLearningStatus.CONTINUE.ToString();
                }

                return;
            }

            if (_pendingStatusList.Contains(MyCourseStatus.MyRegistrationStatus))
            {
                _myClassRunPendingId = GetMyClassRunId(_pendingStatusList);

                if (!string.IsNullOrEmpty(_myClassRunPendingId))
                {
                    MyClassRunName = await GetMyClassRunName(_myClassRunPendingId);
                    GetTitleClassRunInPendingPopup();
                }
            }
            else if (_confirmStatusList.Contains(MyCourseStatus.MyRegistrationStatus))
            {
                string myClassRunId = GetMyClassRunId(_confirmStatusList);

                if (!string.IsNullOrEmpty(myClassRunId))
                {
                    LearningStatus = await GetLearningStatusInConfirmStatus(myClassRunId);
                }
            }
        }

        private void AutomateTracking()
        {
            if (IsMicroLearningType || _myCourseInfo?.Status == null)
            {
                return;
            }

            IsVisiblePostCourse = IsVisiblePostCourseEvaluation() && PermissionHelper.GetPermissionForCheckinDoAssignmentDownloadContentDoPostCourse();
            IsEnablePostCourse = IsVisiblePostCourseEvaluation();

            // Enable post course evaluation
            if (_myClassRun?.PostCourseEvaluationFormCompleted != null && _myClassRun.PostCourseEvaluationFormCompleted.Value)
            {
                IsEnablePostCourse = false;
            }

            AutomateSetLearningStatus();
        }

        private bool IsVisiblePostCourseEvaluation()
        {
            return _myCourseInfo.Status == StatusLearning.Passed.ToString() ||
                   _myCourseInfo.Status == StatusLearning.Failed.ToString() ||
                   _myCourseInfo.Status == StatusLearning.Completed.ToString();
        }

        private void AutomateSetLearningStatus()
        {
            if (_myCourseInfo.Status == StatusLearning.Failed.ToString())
            {
                MyCourseStatus = new MyCourseStatus
                {
                    Status = StatusLearning.Incomplete.ToString(),
                    MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                    MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                    MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                    IsNominated = MyCourseStatus.IsNominated,
                    IsAddToPlan = MyCourseStatus.IsAddToPlan,
                    IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                    ClassRunStatus = MyCourseStatus.ClassRunStatus,
                    IsVisibleLearningStatus = false,
                    LearningStatus = MyCourseStatus.LearningStatus,
                    IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                    IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                };
            }
            else if (_myCourseInfo.Status == StatusLearning.Passed.ToString())
            {
                if (_myClassRun?.PostCourseEvaluationFormCompleted != null && _myClassRun.PostCourseEvaluationFormCompleted.Value)
                {
                    MyCourseStatus = new MyCourseStatus
                    {
                        Status = StatusLearning.Completed.ToString(),
                        MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                        IsNominated = MyCourseStatus.IsNominated,
                        IsAddToPlan = MyCourseStatus.IsAddToPlan,
                        IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                        ClassRunStatus = MyCourseStatus.ClassRunStatus,
                        IsVisibleLearningStatus = false,
                        LearningStatus = MyCourseStatus.LearningStatus,
                        IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                        IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                    };
                }
                else
                {
                    MyCourseStatus = new MyCourseStatus
                    {
                        Status = StatusLearning.InProgress.ToString(),
                        MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                        IsNominated = MyCourseStatus.IsNominated,
                        IsAddToPlan = MyCourseStatus.IsAddToPlan,
                        IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                        ClassRunStatus = MyCourseStatus.ClassRunStatus,
                        IsVisibleLearningStatus = MyCourseStatus.IsVisibleLearningStatus,
                        LearningStatus = MyCourseStatus.LearningStatus,
                        IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                        IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                    };
                }
            }
            else if (_myCourseInfo.Status == StatusLearning.Completed.ToString())
            {
                MyCourseStatus = new MyCourseStatus
                {
                    Status = StatusLearning.Completed.ToString(),
                    MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                    MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                    MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                    IsNominated = MyCourseStatus.IsNominated,
                    IsAddToPlan = MyCourseStatus.IsAddToPlan,
                    IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                    ClassRunStatus = MyCourseStatus.ClassRunStatus,
                    IsVisibleLearningStatus = _isVisibleLearningStatus,
                    LearningStatus = MyCourseStatus.LearningStatus,
                    IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                    IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                };
            }
        }

        private async Task SetStatusOfCourseNotYetStudied()
        {
            _isCourseCompleted = false;
            if (IsMicroLearningType)
            {
                LearningStatus = CourseLearningStatus.START.ToString();
            }
            else
            {
                var pdoInfo = await ExecuteBackendService(() => _pdpmBackendService.GetPDOSInfo(_courseId));
                LearningStatus = pdoInfo.HasEmptyResult() ? CourseLearningStatus.ADDTOPDPLAN.ToString() : CourseLearningStatus.REMOVEFROMPDPLAN.ToString();
            }
        }

        private string GetMyClassRunId(List<string> courseStatus)
        {
            string myClassRunId = string.Empty;

            if (_myClassRun == null)
            {
                return myClassRunId;
            }

            foreach (var status in courseStatus)
            {
                var isExisted = _myClassRun.Status == status;

                if (isExisted)
                {
                    myClassRunId = _myClassRun.ClassRunId;
                }
            }

            return myClassRunId;
        }

        private async Task<string> GetMyClassRunName(string myClassRunPendingId)
        {
            var classRun = await ExecuteBackendService(() => _courseBackendService.GetClassRunById(myClassRunPendingId));
            return classRun.HasEmptyResult() ? string.Empty : classRun.Payload.ClassTitle;
        }

        private void GetTitleClassRunInPendingPopup()
        {
            if (_myClassRun.Status.Equals(RegistrationStatus.WaitlistPendingApprovalByLearner.ToString()))
            {
                PendingPopupTitle = TextsResource.WAITLIST_PLACED;
            }
            else
            {
                PendingPopupTitle = TextsResource.WAITLIST_OFFERED;
            }
        }

        private async Task OnMoreTapped()
        {
            await _stressActionHandler.RunAsync(async () =>
            {
                var isBookmark = _myCourseSummary.BookmarkInfo == null;
                var stringCopyOption = TextsResource.COPY_URL;
                var stringBookmarkOption = isBookmark ? TextsResource.ADD_BOOKMARK : TextsResource.REMOVE_BOOKMARK;
                var stringShareOption = TextsResource.SHARE_OPTION;

                var items = new Dictionary<string, string>();

                if (_isVisibleShare && _isVisibleCopy)
                {
                    items.Add(stringShareOption, "social_share.svg");
                    items.Add(stringCopyOption, "copy.svg");
                }

                if (_isVisibleBookmark)
                {
                    items.Add(stringBookmarkOption, isBookmark ? "bookmark.svg" : "bookmarked.svg");
                }

                if (items.IsNullOrEmpty())
                {
                    return;
                }

                await DialogService.ShowDropDownSelectionPopup(items, isSeparateStringByUppercase: false, onSelected: async option =>
                {
                    if (string.IsNullOrEmpty(option))
                    {
                        return;
                    }

                    if (option == stringCopyOption)
                    {
                        await Clipboard.SetTextAsync(GetDeepLink.GetCourseDeepLink(_courseId));
                        CrossToastPopUp.Current.ShowToastSuccess(TextsResource.COPY_SUCCESSFULLY);
                    }
                    else if (option == stringBookmarkOption)
                    {
                        _myCourseSummary.BookmarkInfo = await _commonService.Bookmark(_courseId, _cardType, isBookmark);
                        CourseBookmarkMessagingCenter.Send(this, new CourseBookmarkMessagingCenterArgs()
                        {
                            CourseId = _courseId,
                            IsBookmarked = isBookmark
                        });
                    }
                    else if (option == stringShareOption)
                    {
                        await NavigationService.NavigateToAsync<SharingContentFormViewModel>(
                            SharingContentFormViewModel.GetNavigationParameters(this));
                    }
                });
            });
        }

        private async Task GetTableOfContent(string classRunId = "")
        {
            if (_courseExtendedInformation.ContentStatus != StatusCourse.Published.ToString())
            {
                _isTableOfContentEmpty = true;

                if (_myCourseInfo?.Status == StatusLearning.Completed.ToString())
                {
                    _isVisibleLearningStatus = false;
                }

                SetMyCourseStatusByTableOfContent();
            }

            // Get content of course or content of class run
            ApiResponse<List<TableOfContent>> result;

            if (IsMicroLearningType)
            {
                result = await ExecuteBackendService(() => _courseBackendService.GetTableOfContentByCourseId(_courseId));
            }
            else
            {
                if ((_myClassRun.Status != null && !_ignoreStatusList.Contains(_myClassRun.Status))
                    || ((_myClassRun.WithdrawalStatus != WithdrawalStatus.Withdrawn.ToString()) && _myClassRun.WithdrawalStatus != null)
                    || !string.IsNullOrEmpty(classRunId))
                {
                    if (!string.IsNullOrEmpty(classRunId))
                    {
                        result = await ExecuteBackendService(() => _courseBackendService.GetTableOfContentByClassRunId(_courseId, classRunId));
                    }
                    else
                    {
                        result = await ExecuteBackendService(() => _courseBackendService.GetTableOfContentByClassRunId(_courseId, _myClassRun.ClassRunId));
                    }
                }
                else
                {
                    result = await ExecuteBackendService(() => _courseBackendService.GetTableOfContentByCourseId(_courseId));
                }
            }

            if (result.HasEmptyResult() && IsMicroLearningType)
            {
                return;
            }

            if ((result.HasEmptyResult() || result.Payload.IsNullOrEmpty()) && !IsMicroLearningType)
            {
                result = await ExecuteBackendService(() => _courseBackendService.GetTableOfContentByCourseId(_courseId));

                if (result.HasEmptyResult())
                {
                    return;
                }
            }

            TableOfContentData = result.Payload;

            if (TableOfContentData.IsNullOrEmpty())
            {
                _isTableOfContentEmpty = true;
                SetMyCourseStatusByTableOfContent();
                return;
            }

            _isTableOfContentEmpty = false;

            TableOfContentData.RemoveAll(toc => toc.Type == TableOfContentType.Assignment);

            MyTableOfContentData = !_myCourseSummary.MyLecturesInfo.IsNullOrEmpty() ? _myCourseSummary.MyLecturesInfo : new List<MyLecturesInfo>();

            _lectures = new List<TableOfContent>();

            foreach (TableOfContent toc in TableOfContentData)
            {
                switch (toc.Type)
                {
                    case TableOfContentType.Assignment:
                        continue;
                    case TableOfContentType.Section:
                        {
                            if (toc.Items != null)
                            {
                                _lectures.AddRange(toc.Items);
                            }

                            break;
                        }

                    default:
                        _lectures.Add(toc);
                        break;
                }
            }

            var currentReviewResponse = await ExecuteBackendService(() => _learnerBackendService.GetCurrentUserReview(_courseId, PdActivityType.Courses));

            if (!currentReviewResponse.HasEmptyResult())
            {
                _ownReview = currentReviewResponse.Payload;
            }

            LearningContentTransferData = new LearningContentTransfer
            {
                Lectures = _lectures,
                CourseId = _courseId,
                MyCourseId = _myCourseId,
                IsCourseCompleted = _isCourseCompleted,
                ThumbnailUrl = _courseExtendedInformation.ThumbnailUrl,
                CourseName = CourseName,
                OwnReview = _ownReview
            };

            SetMyCourseStatusByTableOfContent();
        }

        private void SetMyCourseStatusByTableOfContent()
        {
            if (LearningStatus == CourseLearningStatus.CONTINUE.ToString())
            {
                _isVisibleLearningStatus = !_isTableOfContentEmpty;
            }

            MyCourseStatus = new MyCourseStatus
            {
                Status = MyCourseStatus.Status,
                MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                IsNominated = MyCourseStatus.IsNominated,
                IsAddToPlan = MyCourseStatus.IsAddToPlan,
                IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                ClassRunStatus = MyCourseStatus.ClassRunStatus,
                IsVisibleLearningStatus = _isVisibleLearningStatus,
                LearningStatus = IsMicroLearningType ? LearningStatus : MyCourseStatus.LearningStatus,
                IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                IsTableOfContentEmpty = _isTableOfContentEmpty
            };
        }

        private async Task GetReview()
        {
            // Get list reviews of course
            var reviewResponse = await ExecuteBackendService(() => _learnerBackendService.GetUserReviews(_courseId, PdActivityType.Courses));

            if (reviewResponse.HasEmptyResult() || !reviewResponse.Payload.Items.Any())
            {
                IsVisibleReviewStack = false;
            }

            IsVisibleReviewStack = true;

            foreach (var item in reviewResponse.Payload.Items)
            {
                item.IsOwnerReview = item.UserId == _userId;
            }

            IsVisibleReviewStack = !reviewResponse.Payload.Items.IsNullOrEmpty();

            EditReview = new EditReview
            {
                UserReviews = reviewResponse.Payload.Items,
                IsMicrolearningType = IsMicroLearningType
            };
        }

        private async Task Refresh()
        {
            _isTableOfContentEmpty = false;
            await InitScreen();
        }

        private async Task InitScreen(bool isInit = true)
        {
            if (isInit)
            {
                IsRefreshing = true;
            }

            try
            {
                if (isInit)
                {
                    await GetUserInformation();
                    await GetCourseBasicInformation();
                }

                await GetCourseExtraInformation();
                await GetTableOfContent();
                AutomateTracking();
                SettingAccessRight();
                await GetReview();

                SetLearningOpportunityClassRunCardTransfer();
            }
            finally
            {
                if (isInit)
                {
                    IsRefreshing = false;
                }
            }
        }

        private async Task SetLearningStatusByCurrentYear()
        {
            var pdoInfo = await ExecuteBackendService(() => _pdpmBackendService.GetPDOSInfo(courseId: _courseId));

            if (LearningStatus != CourseLearningStatus.CONTINUE.ToString() && LearningStatus != ViewAgain)
            {
                LearningStatus = pdoInfo.HasEmptyResult() ? CourseLearningStatus.ADDTOPDPLAN.ToString() : CourseLearningStatus.REMOVEFROMPDPLAN.ToString();
            }

            _isAddToPlan = pdoInfo.HasEmptyResult();

            MyCourseStatus = new MyCourseStatus
            {
                Status = MyCourseStatus.Status,
                MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                IsNominated = MyCourseStatus.IsNominated,
                IsAddToPlan = _isAddToPlan,
                IsMicroLearningType = _isMicroLearningType,
                ClassRunStatus = MyCourseStatus.ClassRunStatus,
                IsVisibleLearningStatus = _isVisibleLearningStatus,
                LearningStatus = _learningStatus,
                IsCourseCompleted = _isCourseCompleted,
                IsTableOfContentEmpty = _isTableOfContentEmpty
            };

            if (pdoInfo.HasEmptyResult())
            {
                return;
            }

            if (pdoInfo.Payload.Items.IsNullOrEmpty())
            {
                return;
            }

            _resultId = pdoInfo.Payload.Items.FirstOrDefault()?.ResultIdentity?.ExtId;
        }

        private void SetLearningOpportunityCardTransfer(List<MetadataTag> metadataTags, CourseExtendedInformation courseInfo, List<PrerequisiteCourse> prerequisiteCourses, List<Department> departments, List<UserInformation> collaborativeContentCreatorIds)
        {
            LearningOpportunityCardTransfer = new LearningOpportunityCardTransfer
            {
                MetadataTags = metadataTags,
                CourseExtendedInformation = courseInfo,
                PrerequisiteCourses = prerequisiteCourses,
                Departments = departments,
                UserInformations = collaborativeContentCreatorIds
            };
        }

        private void SetLearningOpportunityClassRunCardTransfer()
        {
            RegistrationMethod registrationMethod = _courseExtendedInformation.RegistrationMethod ?? RegistrationMethod.None;
            bool reachMaximumComplete = _myCourseSummary.CompletedTimes == _courseExtendedInformation.MaxReLearningTimes;

            LearningOpportunityClassRunCardTransfer = new LearningOpportunityClassRunCardTransfer
            {
                CourseId = _courseId,
                UserId = _userId,
                ApprovingOfficer = _approvingOfficerId,
                AlternativeApprovingOfficer = _alternativeApprovingOfficerId,
                MyClassRun = _myClassRun,
                MyClassRuns = _myClassRuns,
                MyCourseStatus = MyCourseStatus,
                RegistrationMethod = registrationMethod,
                IsReachMaximumComplete = reachMaximumComplete,
                RejectedMyClassRuns = _myCourseSummary?.RejectedMyClassRuns,
                WithdrawnMyClassRuns = _myCourseSummary?.WithdrawnMyClassRuns
            };
        }

        private async Task GetUserInformation()
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties == null)
            {
                return;
            }

            _userId = accountProperties.User.Sub;

            var userManagement = await ExecuteBackendService(() => _userManagementBackendService.GetUserManagement(_userId));

            if (userManagement.HasEmptyResult())
            {
                return;
            }

            var userInformationCollection = userManagement.Payload.Items;

            if (userInformationCollection.IsNullOrEmpty())
            {
                return;
            }

            var userGroupCollection = userInformationCollection.Where(user => user.Groups != null).SelectMany(user => user.Groups);
            var groupUserManagements = userGroupCollection as GroupUserManagement[] ?? userGroupCollection.ToArray();

            if (groupUserManagements.IsNullOrEmpty())
            {
                return;
            }

            var primaryApprovalGroup = groupUserManagements.FirstOrDefault(user => user.Type.Equals("PrimaryApprovalGroup"));

            if (primaryApprovalGroup != null)
            {
                _approvingOfficerId = primaryApprovalGroup.UserIdentity?.ExtId;
            }

            var alternativeApprovalGroup = groupUserManagements.FirstOrDefault(user => user.Type.Equals("AlternativeApprovalGroup"));

            if (alternativeApprovalGroup != null)
            {
                _alternativeApprovingOfficerId = alternativeApprovalGroup.UserIdentity?.ExtId;
            }
        }

        private async Task DeclineClassRun()
        {
            await ChangeStatusByCourseClassRun(_myClassRunPendingId, _courseId, RegistrationStatus.WaitlistRejected, RegistrationStatus.OfferRejected);
        }

        private async Task AcceptClassRun()
        {
            await ChangeStatusByCourseClassRun(_myClassRunPendingId, _courseId, RegistrationStatus.WaitlistConfirmed, RegistrationStatus.OfferConfirmed);
        }

        private async Task ChangeStatusByCourseClassRun(string myClassRunId, string courseId, RegistrationStatus waitlist, RegistrationStatus offer)
        {
            if (_pendingStatusList.Contains(MyCourseStatus.MyRegistrationStatus))
            {
                string status = MyCourseStatus.MyRegistrationStatus.Equals(RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
                                        ? waitlist.ToString()
                                        : offer.ToString();

                var changeStatusApprove = new
                {
                    ClassRunId = myClassRunId,
                    CourseId = courseId,
                    Status = status
                };

                await ExecuteBackendService(() => _courseBackendService.ChangeStatusByCourseClassRun(changeStatusApprove));

                if (_myClassRuns.Count == 2)
                {
                    var classRunPrevious = _myClassRuns.FirstOrDefault(run => run.RegistrationType == RegistrationType.Manual);

                    if (classRunPrevious != null)
                    {
                        _myClassRun = classRunPrevious;

                        string myStatusPrevious = string.IsNullOrEmpty(classRunPrevious.WithdrawalStatus) ? classRunPrevious.Status : classRunPrevious.WithdrawalStatus;

                        MyCourseStatus = new MyCourseStatus
                        {
                            Status = MyCourseStatus.Status,
                            MyRegistrationStatus = myStatusPrevious,
                            MyWithdrawalStatus = classRunPrevious.WithdrawalStatus,
                            MyCourseDisplayStatus = myStatusPrevious,
                            IsNominated = false,
                            IsAddToPlan = _isAddToPlan,
                            IsMicroLearningType = _isMicroLearningType,
                            ClassRunStatus = ClassRunInProgress?.Status ?? ClassRunStatus.None,
                            IsVisibleLearningStatus = _isVisibleLearningStatus,
                            LearningStatus = _learningStatus,
                            IsCourseCompleted = _isCourseCompleted,
                            IsTableOfContentEmpty = _isTableOfContentEmpty
                        };
                    }
                }
                else
                {
                    MyCourseStatus = new MyCourseStatus
                    {
                        Status = MyCourseStatus.Status,
                        MyRegistrationStatus = status,
                        MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = status,
                        IsNominated = MyCourseStatus.IsNominated,
                        IsAddToPlan = _isAddToPlan,
                        IsMicroLearningType = _isMicroLearningType,
                        ClassRunStatus = ClassRunInProgress?.Status ?? ClassRunStatus.None,
                        IsVisibleLearningStatus = _isVisibleLearningStatus,
                        LearningStatus = _learningStatus,
                        IsCourseCompleted = _isCourseCompleted,
                        IsTableOfContentEmpty = _isTableOfContentEmpty
                    };
                }

                if (status.Equals(RegistrationStatus.OfferConfirmed.ToString()))
                {
                    LearningStatus = await GetLearningStatusInConfirmStatus(myClassRunId);
                }

                var myCourseSummaryInfo = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(_courseId));

                if (myCourseSummaryInfo.HasEmptyResult())
                {
                    return;
                }

                _myCourseSummary = myCourseSummaryInfo.Payload;

                SetLearningOpportunityClassRunCardTransfer();
            }
        }

        private async Task<string> GetLearningStatusInConfirmStatus(string myClassRunId)
        {
            var classRun = await ExecuteBackendService(() => _courseBackendService.GetClassRunById(myClassRunId));

            if (classRun.HasEmptyResult())
            {
                return LearningStatus;
            }

            var run = classRun.Payload;

            if (run == null)
            {
                return LearningStatus;
            }

            if (DateTime.Now.ToUniversalTime().Date >= run.GetStartDateTime().Date && !_isCourseCompleted && LearningStatus != CourseLearningStatus.CONTINUE.ToString())
            {
                LearningStatus = CourseLearningStatus.START.ToString();

                MyCourseStatus = new MyCourseStatus
                {
                    MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                    Status = MyCourseStatus.Status,
                    MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                    MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                    IsNominated = MyCourseStatus.IsNominated,
                    IsAddToPlan = _isAddToPlan,
                    IsMicroLearningType = _isMicroLearningType,
                    ClassRunStatus = ClassRunInProgress?.Status ?? ClassRunStatus.None,
                    IsVisibleLearningStatus = true,
                    LearningStatus = _learningStatus,
                    IsCourseCompleted = _isCourseCompleted,
                    IsTableOfContentEmpty = _isTableOfContentEmpty
                };

                return LearningStatus;
            }

            MyCourseStatus = new MyCourseStatus
            {
                MyRegistrationStatus = RegistrationStatus.ConfirmedBeforeStartDate.ToString(),
                Status = MyCourseStatus.Status,
                MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                IsNominated = MyCourseStatus.IsNominated,
                IsAddToPlan = _isAddToPlan,
                IsMicroLearningType = _isMicroLearningType,
                ClassRunStatus = ClassRunInProgress?.Status ?? ClassRunStatus.None,
                IsVisibleLearningStatus = _isVisibleLearningStatus,
                LearningStatus = _learningStatus,
                IsCourseCompleted = _isCourseCompleted,
                IsTableOfContentEmpty = _isTableOfContentEmpty
            };

            await SetLearningStatusByCurrentYear();

            return LearningStatus;
        }

        private async Task OpenPostCourse()
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("formId", _courseExtendedInformation.PostCourseEvaluationFormId);
            parameters.SetParameter("resourceId", _courseId);
            parameters.SetParameter("registrationId", _myClassRun.RegistrationId);
            await NavigationService.NavigateToAsync<PostCourseViewModel>(parameters);
        }

        private async Task GetMyAssignments(ParticipantAssignmentTracksRequest participantAssignmentTracksRequest)
        {
            AssignmentDetails = new List<AssignmentDetail>();

            if (participantAssignmentTracksRequest == null)
            {
                return;
            }

            var participantAssignmentTrackRequest = new ParticipantAssignmentTrackRequest
            {
                CourseId = participantAssignmentTracksRequest.CourseId,
                ClassRunId = participantAssignmentTracksRequest.ClassRunId
            };

            var participantAssignmentTracks = await ExecuteBackendService(() => _courseBackendService.GetParticipantAssignmentTracks(participantAssignmentTrackRequest));

            if (participantAssignmentTracks.HasEmptyResult())
            {
                return;
            }

            var assignmentLearnerResponse = await ExecuteBackendService(() => _learnerBackendService.GetMyAssignmentsByRegistrationId(participantAssignmentTracksRequest.RegistrationId));

            if (assignmentLearnerResponse.HasEmptyResult())
            {
                return;
            }

            var assignmentTracks = participantAssignmentTracks.Payload.Items;
            var assignmentLearners = assignmentLearnerResponse.Payload.Items;

            if (assignmentTracks.IsNullOrEmpty())
            {
                return;
            }

            var assignmentIds = assignmentTracks.Select(ass => ass.AssignmentId).ToArray();

            if (assignmentIds.IsNullOrEmpty())
            {
                return;
            }

            var assignmentRequest = new AssignmentRequest { Ids = assignmentIds, IncludeQuizForm = false };

            var assignmentInfoResponse = await ExecuteBackendService(() => _courseBackendService.GetMyAssignmentsByAssignmentIds(assignmentRequest));

            if (assignmentInfoResponse.HasEmptyResult())
            {
                return;
            }

            var assignmentInfos = assignmentInfoResponse.Payload;
            var participantIds = assignmentTracks.Select(ass => ass.Id).ToArray();
            var participantAssignmentTrackResponse = await ExecuteBackendService(() => _courseBackendService.ParticipantAssignmentTrackByIds(participantIds));
            var participantAssignmentTrackInfos = participantAssignmentTrackResponse.Payload;

            var assignmentDetails = new List<AssignmentDetail>();
            foreach (var assignmentTrack in participantAssignmentTrackInfos)
            {
                var assignmentInfo = assignmentInfos.FirstOrDefault(x => x.Id == assignmentTrack.AssignmentId);
                var assignmentLearner =
                    assignmentLearners.FirstOrDefault(x => x.AssignmentId == assignmentTrack.AssignmentId);

                assignmentDetails.Add(AssignmentDetail.CreateFrom(assignmentLearner, assignmentInfo, assignmentTrack));
            }

            AssignmentDetails = assignmentDetails;
            var lastItem = AssignmentDetails.LastOrDefault();

            if (lastItem != null)
            {
                lastItem.IsLastItem = true;
            }
        }

        private async Task<List<MetadataTag>> GetMetadataTagging()
        {
            var metadataTagging = Application.Current.Properties.GetMetadataTagging();

            if (!metadataTagging.IsNullOrEmpty())
            {
                return metadataTagging;
            }

            var tagging = await ExecuteBackendService(() => _metadataBackendService.GetMetadata());

            if (tagging.HasEmptyResult())
            {
                return metadataTagging;
            }

            Application.Current.Properties.AddMetadataTagging(tagging.Payload);
            metadataTagging = Application.Current.Properties.GetMetadataTagging();

            return metadataTagging;
        }

        private void OnEditUserComment(UserReview review)
        {
            DialogService.ShowEditCommentPopup(review, IsMicroLearningType, async userReview =>
            {
                var courseReview = new
                {
                    ItemId = _myCourseSummary.CourseId,
                    Rating = (int)review.Rate,
                    review.CommentContent,
                    ItemType = "Course"
                };

                // update comment
                var result = await ExecuteBackendService(() => _learnerBackendService.UpdateUserReview(_myCourseSummary.CourseId, courseReview));

                if (!result.IsError)
                {
                    await GetReview();
                    CrossToastPopUp.Current.ShowToastSuccess(IsMicroLearningType
                        ? TextsResource.FEEDBACK_MLU_UPDATED
                        : TextsResource.FEEDBACK_UPDATED);
                    await InitScreen();
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError(IsMicroLearningType
                        ? TextsResource.FEEDBACK_MLU_FAILED
                        : TextsResource.FEEDBACK_FAILED);
                }
            });
        }

        private async Task OpenCourseCommunity()
        {
            var courseResponse = await ExecuteBackendService(() => _courseBackendService.GetCourseByCourseId(_courseId));
            if (courseResponse.IsError || courseResponse.HasEmptyResult())
            {
                return;
            }

            string redirectUrl = courseResponse.Payload.ContentStatus == nameof(StatusCourse.Unpublished)
                ? $"{GlobalSettings.WebViewUrlSocial}/s/course-{_courseId}"
                : $"{GlobalSettings.WebViewUrlSocial}/s/{_courseCode}";

            var loginResponse = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(redirectUrl));
            if (loginResponse.IsError || loginResponse.HasEmptyResult())
            {
                return;
            }

            var parameters = new NavigationParameters();
            parameters.SetParameter("IdmResponse", loginResponse.Payload);
            parameters.SetParameter("ReturnUrl", redirectUrl);
            await this.NavigationService.NavigateToAsync<CommunityViewModel>(parameters);
        }

        private async Task LikeTapped()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                using (DialogService.DisplayLoadingIndicator())
                {
                    var likeResponse = await ExecuteBackendService(() => _learnerBackendService.Like(new
                    {
                        itemId = _courseId,
                        itemType = "Course",
                        isLike = !LearningOpportunityInformationCardTransfer.CourseExtendedInformation.IsLike
                    }));

                    if (!likeResponse.HasEmptyResult())
                    {
                        LearningOpportunityInformationCardTransfer.CourseExtendedInformation.IsLike = likeResponse.Payload.IsLike;
                        LearningOpportunityInformationCardTransfer.CourseExtendedInformation.TotalLike = likeResponse.Payload.TotalLike;
                        RaisePropertyChanged(() => LearningOpportunityInformationCardTransfer);
                    }

                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// Setting general permsission: bookmark, like, share, copy.
        /// </summary>
        private void SettingAccessRight()
        {
            // Bookmark
            _isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();

            bool isShowLikeShareCopy = IsMicroLearningType
                ? PermissionHelper.GetPermissionForLikeShareCopy(CardType.Microlearning)
                : PermissionHelper.GetPermissionForLikeShareCopy(CardType.Course);

            IsVisibleLike = _isVisibleCopy = _isVisibleShare = isShowLikeShareCopy;

            if (!_isVisibleBookmark && !isShowLikeShareCopy)
            {
                IsVisibleMore = false;
            }

            // Add course to PD Plan, remove course from PD Plan
            if (LearningStatus == CourseLearningStatus.ADDTOPDPLAN.ToString() || LearningStatus == CourseLearningStatus.REMOVEFROMPDPLAN.ToString())
            {
                if (!PermissionHelper.GetPermissionForAddRemovePlan())
                {
                    MyCourseStatus = new MyCourseStatus
                    {
                        Status = MyCourseStatus.Status,
                        MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                        IsNominated = MyCourseStatus.IsNominated,
                        IsAddToPlan = MyCourseStatus.IsAddToPlan,
                        IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                        ClassRunStatus = MyCourseStatus.ClassRunStatus,
                        IsVisibleLearningStatus = false,
                        LearningStatus = MyCourseStatus.LearningStatus,
                        IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                        IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                    };
                }
            }
            else if (LearningStatus == CourseLearningStatus.START.ToString())
            {
                if (!PermissionHelper.GetPermissionForStartLearning())
                {
                    MyCourseStatus = new MyCourseStatus
                    {
                        Status = MyCourseStatus.Status,
                        MyRegistrationStatus = MyCourseStatus.MyRegistrationStatus,
                        MyWithdrawalStatus = MyCourseStatus.MyWithdrawalStatus,
                        MyCourseDisplayStatus = MyCourseStatus.MyCourseDisplayStatus,
                        IsNominated = MyCourseStatus.IsNominated,
                        IsAddToPlan = MyCourseStatus.IsAddToPlan,
                        IsMicroLearningType = MyCourseStatus.IsMicroLearningType,
                        ClassRunStatus = MyCourseStatus.ClassRunStatus,
                        IsVisibleLearningStatus = false,
                        LearningStatus = MyCourseStatus.LearningStatus,
                        IsCourseCompleted = MyCourseStatus.IsCourseCompleted,
                        IsTableOfContentEmpty = MyCourseStatus.IsTableOfContentEmpty
                    };
                }
            }
        }
    }
}
