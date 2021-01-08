using System;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        private readonly ICourseBackendService _courseBackendService;

        private readonly IOrganizationBackendService _organizationBackendService;

        private readonly ICommonServices _commonService;

        public ListViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
        }

        ~ListViewModel()
        {
            MessagingCenter.Unsubscribe<ListViewModel, ItemCard>(this, "bookmarked-item-on-list");
        }

        public async Task ItemBookmarked(ItemCard data)
        {
            switch (data.CardType)
            {
                case BookmarkType.Microlearning:
                case BookmarkType.Course:
                    if (data != null && !data.IsExpired && !data.CourseStatus.Equals(nameof(StatusCourse.Unpublished)) && !data.CourseStatus.Equals(nameof(StatusLearning.Archived)))
                    {
                        data.BookmarkInfo = await _commonService.Bookmark(data.Id, data.CardType, data.BookmarkInfo == null);
                    }

                    MessagingCenter.Send(this, "bookmarked-item-on-list", data);
                    break;
                case BookmarkType.Community:
                    data.BookmarkInfo = await _commonService.Bookmark(data.Guid, BookmarkType.Community, data.BookmarkInfo == null);
                    break;
                case BookmarkType.LearningPath:
                    data.BookmarkInfo = await _commonService.Bookmark(data.Id, BookmarkType.LearningPath, data.BookmarkInfo == null);
                    break;
                default:
                    break;
            }
        }

        public async Task ItemSelected(ItemCard data)
        {
            var navParams = new NavigationParameters();
            navParams.SetParameter("course-card", data);

            if (data.CardType == BookmarkType.Course || data.CardType == BookmarkType.Microlearning)
            {
                if (!data.IsExpired && !data.CourseStatus.Equals(nameof(StatusCourse.Unpublished)) && !data.CourseStatus.Equals(nameof(StatusLearning.Archived)))
                {
                    await NavigationService.NavigateToAsync<CourseDetailsViewModel>(CourseDetailsViewModel.GetNavigationParameters(data.Id, data.CardType));
                }
            }
            else
            {
                await NavigationService.NavigateToAsync<MyDigitalContentDetailsViewModel>(MyDigitalContentDetailsViewModel.GetNavigationParameters(data.Id));
            }
        }

        public async void AccessOffer_Clicked(object sender, EventArgs e)
        {
            var param = (sender as Button)?.CommandParameter as ItemCard;

            if (param != null && param.MyClassRun.Status == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.WaitlistConfirmed);
            }
            else
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.OfferConfirmed);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                (((sender as Button).Parent as StackLayout).Parent as StackLayout).IsVisible = false;
            });
        }

        public async void DeclineOffer_Clicked(object sender, EventArgs e)
        {
            var param = (sender as Button)?.CommandParameter as ItemCard;

            if (param != null && param.MyClassRun.Status == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.WaitlistRejected);
            }
            else
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.OfferRejected);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                (((sender as Button).Parent as StackLayout).Parent as StackLayout).IsVisible = false;
            });
        }

        public async void ShowRejectComment_Clicked(object sender, EventArgs e)
        {
            var data = (sender as Button)?.CommandParameter as ItemCard;

            var userInfor = await GetUserInfo(data.MyClassRun.ChangedBy);

            if (!string.IsNullOrEmpty(data.MyClassRunRejectReason))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DialogService.ShowMyClassRunRejectReasonPopup(userInfor, data.MyClassRun.ChangedDate, data.MyClassRunRejectReason);
                });
            }
        }

        public async Task ShowRejectComment(ItemCard data)
        {
            var userInfor = await GetUserInfo(data.MyClassRun.ChangedBy);

            if (!string.IsNullOrEmpty(data.MyClassRunRejectReason))
            {
                await DialogService.ShowMyClassRunRejectReasonPopup(userInfor, data.MyClassRun.ChangedDate, data.MyClassRunRejectReason);
            }
        }

        public async Task AccessOffer(ItemCard param)
        {
            if (param != null && param.MyClassRun.Status == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.WaitlistConfirmed);
            }
            else
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.OfferConfirmed);
            }
        }

        public async Task DeclineOffer(ItemCard param)
        {
            if (param != null && param.MyClassRun.Status == RegistrationStatus.WaitlistPendingApprovalByLearner.ToString())
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.WaitlistRejected);
            }
            else
            {
                await ChangeStatusByCourseClassRun(param.MyClassRun.ClassRunId, param.Id, RegistrationStatus.OfferRejected);
            }
        }

        private async Task ChangeStatusByCourseClassRun(string myClassRunId, string courseId, RegistrationStatus offer)
        {
            await ExecuteBackendService(() => _courseBackendService.ChangeStatusByCourseClassRun(new { ClassRunId = myClassRunId, CourseId = courseId, Status = offer.ToString() }));
        }

        private async Task<UserInformation> GetUserInfo(string userId)
        {
            var userResponse = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(new { UserCxIds = new string[] { userId } }));

            if (userResponse.HasEmptyResult())
            {
                return null;
            }

            return userResponse.Payload.FirstOrDefault();
        }
    }
}
