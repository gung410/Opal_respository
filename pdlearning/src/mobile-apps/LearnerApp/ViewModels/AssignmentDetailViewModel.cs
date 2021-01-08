using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.Course;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.ViewModels.MyLearning;
using LearnerApp.Views.MyLearning;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class AssignmentDetailViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ICourseBackendService _courseBackendService;
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;

        private string _title;
        private string _description;
        private string _comment;
        private string _totalComment;
        private bool _isVisibleListComment;
        private bool _isVisibleDoAssignment;
        private string _avatar;
        private ParticipantAssignmentTrackStatus _status;
        private DateTime _startDate;
        private DateTime _endDate;
        private QuizAnswer _quizAnswer;
        private AssignmentDetail _assignmentDetail;

        private bool _isRefreshing = true;

        public AssignmentDetailViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);

            MessagingCenter.Subscribe<AssignmentLearningView>(this, "on-close-assignment-player", async (sender) =>
            {
                using (DialogService.DisplayLoadingIndicator())
                {
                    // Get newest status for assignment
                    var listAssignmentsByRegistrationId = await ExecuteBackendService(() => _learnerBackendService.GetMyAssignmentsByRegistrationId(_assignmentDetail.RegistrationId));
                    if (!listAssignmentsByRegistrationId.HasEmptyResult() && !listAssignmentsByRegistrationId.IsError)
                    {
                        var currentAssignment = listAssignmentsByRegistrationId.Payload.Items.FirstOrDefault(p => p.AssignmentId == _assignmentDetail.Id);

                        // Update assignment status
                        if (currentAssignment != null)
                        {
                            Status = currentAssignment.Status;
                            AssignmentDetail.Status = currentAssignment.Status;
                            RaisePropertyChanged(() => AssignmentDetail);
                        }
                    }

                    await InitScreen(true);
                }
            });
        }

        public ObservableCollection<Comment> CommentCollection { get; set; }

        public ICommand PlayAssignmentCommand => new Command(async () => await OnPlayAssignment());

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand AddCommentCommand => new Command(async () => await OnAddCommentComment());

        public string Avatar
        {
            get
            {
                return _avatar;
            }

            set
            {
                _avatar = value;
                RaisePropertyChanged(() => Avatar);
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

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public bool IsVisibleListComment
        {
            get
            {
                return _isVisibleListComment;
            }

            set
            {
                _isVisibleListComment = value;
                RaisePropertyChanged(() => IsVisibleListComment);
            }
        }

        public string TotalComment
        {
            get
            {
                return _totalComment;
            }

            set
            {
                _totalComment = value;
                RaisePropertyChanged(() => TotalComment);
            }
        }

        public ParticipantAssignmentTrackStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
                RaisePropertyChanged(() => StartDate);
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
                RaisePropertyChanged(() => EndDate);
            }
        }

        public QuizAnswer QuizAnswer
        {
            get
            {
                return _quizAnswer;
            }

            set
            {
                _quizAnswer = value;
                RaisePropertyChanged(() => QuizAnswer);
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
                RaisePropertyChanged(() => IsRefreshingForRefreshView);
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

        public string Comment
        {
            get
            {
                return _comment;
            }

            set
            {
                _comment = value;
                RaisePropertyChanged(() => Comment);
            }
        }

        public AssignmentDetail AssignmentDetail
        {
            get
            {
                return _assignmentDetail;
            }

            set
            {
                _assignmentDetail = value;
                RaisePropertyChanged(() => AssignmentDetail);
            }
        }

        public bool IsVisibleDoAssignment
        {
            get
            {
                return _isVisibleDoAssignment;
            }

            set
            {
                _isVisibleDoAssignment = value;
                RaisePropertyChanged(() => IsVisibleDoAssignment);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseDetailsAssignment;

        public Action RequestGoToCommentList { get; set; }

        public static NavigationParameters GetNavigationParameters(string courseId, string assignmentId)
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.SetParameter("course-id", courseId);
            navigationParameter.SetParameter("assignment-id", assignmentId);
            navigationParameter.SetParameter("start-player", true);
            return navigationParameter;
        }

        public static NavigationParameters GetNavigationParameters(
            AssignmentDetail assignmentDetail, bool goToComment = false)
        {
            var navigationParameter = new NavigationParameters();
            navigationParameter.SetParameter("assignment-detail", assignmentDetail);
            navigationParameter.SetParameter("assignment-detail-comment", goToComment);
            return navigationParameter;
        }

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<AssignmentLearningView>(this, "on-close-assignment-player");
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            IsRefreshing = true;
            try
            {
                AssignmentDetail = await FetchAssigmentDetail(navigationParameters);
                Status = AssignmentDetail.Status;

                var isGoToCommentList = navigationParameters.GetParameter<bool>("assignment-detail-comment");
                var startPlayer = navigationParameters.GetParameter<bool>("start-player");

                await InitScreen();

                if (isGoToCommentList)
                {
                    RequestGoToCommentList?.Invoke();
                }

                if (startPlayer)
                {
                    await OnPlayAssignment();
                }
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task<AssignmentDetail> FetchAssigmentDetail(NavigationParameters navigationParameters)
        {
            var assignmentDetail = navigationParameters.GetParameter<AssignmentDetail>("assignment-detail");
            if (assignmentDetail != null)
            {
                return assignmentDetail;
            }

            var assignmentId = navigationParameters.GetParameter<string>("assignment-id");
            var courseId = navigationParameters.GetParameter<string>("course-id");
            if (!string.IsNullOrEmpty(assignmentId) && !string.IsNullOrEmpty(courseId))
            {
                var myCourseSummaryInfo = await ExecuteBackendService(() => _learnerBackendService.GetMyCourseSummary(courseId));
                var myClassRun = myCourseSummaryInfo.Payload.MyClassRuns[0]; // In Progress will always be the first

                return await GetMyAssignments(assignmentId, myClassRun.RegistrationId);
            }

            return null;
        }

        private async Task<AssignmentDetail> GetMyAssignments(
            string assignmentId,
            string registrationId)
        {
            var assignmentLearnerResponse = await ExecuteBackendService(() => _learnerBackendService.GetMyAssignmentsByRegistrationId(registrationId));
            if (assignmentLearnerResponse.HasEmptyResult())
            {
                return null;
            }

            var assignmentLearner
                = assignmentLearnerResponse.Payload.Items.First(
                    learnerResponse => learnerResponse.AssignmentId == assignmentId);

            var participantAssignmentTracks = await ExecuteBackendService(() => _courseBackendService.GetParticipantAssignmentTracksByIds(
                new string[] { assignmentLearner.ParticipantAssignmentTrackId }));
            if (participantAssignmentTracks.HasEmptyResult())
            {
                return null;
            }

            var assignmentTrack = participantAssignmentTracks.Payload.First();

            var assignment = new AssignmentRequest { Ids = new string[] { assignmentId }, IncludeQuizForm = false };

            var assignmentInfoResponse = await ExecuteBackendService(() => _courseBackendService.GetMyAssignmentsByAssignmentIds(assignment));

            if (assignmentInfoResponse.HasEmptyResult())
            {
                return null;
            }

            var assignmentInfo = assignmentInfoResponse.Payload[0];

            return AssignmentDetail.CreateFrom(assignmentLearner, assignmentInfo, assignmentTrack);
        }

        private async Task Refresh()
        {
            await InitScreen(true);

            IsRefreshing = false;
        }

        private async Task InitScreen(bool init = false)
        {
            if (_assignmentDetail == null)
            {
                return;
            }

            if (!init)
            {
                Title = _assignmentDetail.Title;
                StartDate = _assignmentDetail.StartDate;
                EndDate = _assignmentDetail.EndDate;
                QuizAnswer = _assignmentDetail.QuizAnswer;
            }

            IsVisibleDoAssignment = AssignmentDetail.Status == ParticipantAssignmentTrackStatus.NotStarted ||
                    AssignmentDetail.Status == ParticipantAssignmentTrackStatus.InProgress;

            // List of comment of assignment
            await CreateAssignmentReviewed();

            GetUserAvatar();
        }

        private async Task CreateAssignmentReviewed()
        {
            var commentRequest = new CommentRequest
            {
                ObjectId = _assignmentDetail.ParticipantAssignmentTrackId,
                EntityCommentType = EntityCommentType.ParticipantAssignmentTrackQuizAnswer,
                PagedInfo = new PagedResultRequestDto()
            };

            var commentResponse = await ExecuteBackendService(() => _courseBackendService.GetAssignmentComment(commentRequest));

            if (!commentResponse.HasEmptyResult() && commentResponse.Payload.TotalCount > 0)
            {
                var userCxIds = new
                {
                    UserCxIds = commentResponse.Payload.Items.Select(p => p.UserId).ToArray()
                };

                var userInfoResponse = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(userCxIds));

                if (!userInfoResponse.HasEmptyResult() && userInfoResponse.Payload.Count > 0)
                {
                    foreach (var item in commentResponse.Payload.Items)
                    {
                        var info = userInfoResponse.Payload.FirstOrDefault(p => p.UserCxId == item.UserId);

                        if (info != null)
                        {
                            item.AvatarUrl = info.AvatarUrl;
                            item.FullName = info.FullName;
                            item.DepartmentName = info.DepartmentName;
                        }
                    }
                }

                TotalComment = commentResponse.Payload.TotalCount == 1 ? "COMMENT (1)" : $"COMMENTS ({commentResponse.Payload.TotalCount})";

                CommentCollection = new ObservableCollection<Comment>(commentResponse.Payload.Items);

                RaisePropertyChanged(() => CommentCollection);

                IsVisibleListComment = true;
            }
            else
            {
                TotalComment = "COMMENTS (0)";

                IsVisibleListComment = false;
            }
        }

        private async Task OnPlayAssignment()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var startDate = GetDateTimeLocal.Convert(_assignmentDetail.StartDate).Date;
                var now = DateTime.Now.ToUniversalTime().Date;
                var dueDate = _assignmentDetail.EndDate.AddDays(30).Date;

                if (DateTime.Compare(startDate, now) > 0)
                {
                    await DialogService.ShowAlertAsync("You cannot start the assignment prior to its start date.", "Close", isVisibleIcon: false);
                }
                else
                {
                    if (_assignmentDetail.Status == ParticipantAssignmentTrackStatus.NotStarted &&
                        DateTime.Compare(now, dueDate) <= 0)
                    {
                        await ExecuteBackendService(() => _learnerBackendService.UpdateAssignmentStatus(new
                        {
                            AssignmentId = _assignmentDetail.Id,
                            RegistrationId = _assignmentDetail.RegistrationId,
                            Status = ParticipantAssignmentTrackStatus.InProgress
                        }));
                    }

                    await NavigationService.NavigateToAsync<AssignmentLearningViewModel>(
                        AssignmentLearningViewModel.GetNavigationParameters(_assignmentDetail));
                }
            }
        }

        private async Task OnAddCommentComment()
        {
            if (string.IsNullOrEmpty(Comment))
            {
                CrossToastPopUp.Current.ShowToastError("Please fill in your comment");
                return;
            }

            using (DialogService.DisplayLoadingIndicator())
            {
                var createComment = new CreateCommentRequest
                {
                    ObjectId = _assignmentDetail.ParticipantAssignmentTrackId,
                    EntityCommentType = EntityCommentType.ParticipantAssignmentTrackQuizAnswer,
                    Content = Comment
                };

                var createCommentResponse = await ExecuteBackendService(() => _courseBackendService.CreateAssignmentComment(createComment));

                if (!createCommentResponse.IsError && !createCommentResponse.HasEmptyResult())
                {
                    CrossToastPopUp.Current.ShowToastSuccess("Comment sent successfully");

                    await CreateAssignmentReviewed();

                    Comment = string.Empty;
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError("Comment sent failed");
                }
            }
        }

        private void GetUserAvatar()
        {
            var accountProperties = App.Current.Properties.GetAccountProperties();

            if (accountProperties == null)
            {
                return;
            }

            Avatar = $"{GlobalSettings.BackendServiceUserAvatar}/{accountProperties.User.Sub}";
        }
    }
}
