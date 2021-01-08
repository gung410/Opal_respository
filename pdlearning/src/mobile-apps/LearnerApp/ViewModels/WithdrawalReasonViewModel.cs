using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Course;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class WithdrawalReasonViewModel : BasePageViewModel, INavigationAware
    {
        private const int SkipCount = 10;

        private readonly ICourseBackendService _courseBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;

        private int _skipCount;
        private string _title;
        private string _registrationId;
        private bool _isRefreshing;

        public WithdrawalReasonViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
        }

        public LearnerObservableCollection<Comment> CommentCollection { get; set; }

        public int TotalCount { get; set; }

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

        public ICommand RefreshCommand => new Command(async () => await Refresh());

        public ICommand LoadMoreItemCommand => new Command(async () => await GetMoreWithdrawalReasons());

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseDetailsWithdrawal;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                Title = navigationParameters.GetParameter<string>("Title");
                _registrationId = navigationParameters.GetParameter<string>("RegistrationId");
                await InitScreen();
            }
        }

        private async Task Refresh()
        {
            await InitScreen();
            IsRefreshing = false;
        }

        private async Task InitScreen()
        {
            _skipCount = 0;

            var comments = await GetCommentList(_skipCount);
            CommentCollection = new LearnerObservableCollection<Comment>(comments);
            RaisePropertyChanged(() => CommentCollection);
        }

        private async Task GetMoreWithdrawalReasons()
        {
            _skipCount += SkipCount;
            var comments = await GetCommentList(_skipCount);
            CommentCollection.AddRange(comments);
            RaisePropertyChanged(() => CommentCollection);
        }

        private async Task<List<Comment>> GetCommentList(int skipCount)
        {
            var commentRequest = new CommentRequest
            {
                ObjectId = _registrationId,
                ActionType = "withdrawn",
                EntityCommentType = EntityCommentType.Registration,
                PagedInfo = new PagedResultRequestDto
                {
                    SkipCount = skipCount
                }
            };

            var commentResponse = await ExecuteBackendService(() => _courseBackendService.GetAssignmentComment(commentRequest));

            if (commentResponse.HasEmptyResult())
            {
                return null;
            }

            TotalCount = commentResponse.Payload.TotalCount;

            var userCxIds = new
            {
                UserCxIds = commentResponse.Payload.Items.Select(p => p.UserId).ToArray()
            };

            if (userCxIds.UserCxIds.IsNullOrEmpty())
            {
                return null;
            }

            var userInfoResponse = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(userCxIds));

            if (userInfoResponse.HasEmptyResult())
            {
                return null;
            }

            var comments = commentResponse.Payload.Items;

            foreach (var item in comments)
            {
                var info = userInfoResponse.Payload.FirstOrDefault(p => p.UserCxId == item.UserId);

                if (info == null)
                {
                    continue;
                }

                item.AvatarUrl = info.AvatarUrl;
                item.FullName = info.FullName;
                item.DepartmentName = info.DepartmentName;
            }

            return comments;
        }
    }
}
