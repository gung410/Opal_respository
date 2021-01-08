using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Models.OutstandingTask;
using LearnerApp.Services;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Newtonsoft.Json;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class NotificationsViewModel : BasePageViewModel, INavigationAware
    {
        public IdmResponse IdmResponse;
        public string ReturnUrl;

        private readonly ICourseBackendService _courseBackendService;
        private readonly IIdmBackendService _idmBackendService;
        private readonly ICommonServices _commonService;

        private string _sourceUrl;

        public NotificationsViewModel()
        {
            _commonService = DependencyService.Resolve<ICommonServices>();
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
            _idmBackendService = CreateRestClientFor<IIdmBackendService>(GlobalSettings.BackendServiceIdm);
        }

        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                RaisePropertyChanged(() => SourceUrl);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Notifications;

        public async Task GoToItemDetails(string data)
        {
            Notification item = JsonConvert.DeserializeObject<Notification>(data);

            switch (item.ObjectType)
            {
                case OutstandingTaskTypeEnum.Course:
                    if (item == null || string.IsNullOrEmpty(item.ObjectId))
                    {
                        return;
                    }

                    // In this time, we just handle a case is ObjectType = 'course'
                    var courseList = await ExecuteBackendService(() => _courseBackendService.GetCourseListByIdentifiers(new string[] { item.ObjectId }));

                    if (courseList.HasEmptyResult())
                    {
                        return;
                    }

                    var courseCard = await _commonService.CreateCourseCardList(courseList.Payload);

                    if (courseCard.IsNullOrEmpty())
                    {
                        return;
                    }

                    ItemCard course = courseCard[0];

                    if (!course.IsExpired && !course.CourseStatus.Equals(nameof(StatusCourse.Unpublished)))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            var navParams = new NavigationParameters();
                            navParams.SetParameter("course-card", course);
                            await NavigationService.NavigateToAsync<CourseDetailsViewModel>(navParams);
                        });
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError("Course expired or unpublished");
                    }

                    break;
                case OutstandingTaskTypeEnum.StandaloneForm:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (!string.IsNullOrEmpty(item.ObjectId))
                        {
                            var navigationParameter = new NavigationParameters();
                            navigationParameter.SetParameter("form-id", item.ObjectId);
                            NavigationService.NavigateToAsync<StandAloneFormViewModel>(navigationParameter);
                        }
                    });

                    break;
                default:
                    break;
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            ReturnUrl = $"{GlobalSettings.WebViewUrlNotification}?isMobile=true";

            var apiResult = await ExecuteBackendService(() => _idmBackendService.GetLoginWithTokenUrl(ReturnUrl));

            if (apiResult.IsError || apiResult.HasEmptyResult())
            {
                return;
            }

            IdmResponse = apiResult.Payload;

            SourceUrl = $"{GlobalSettings.BackendServiceIdm}/DirectLogin.html";
        }
    }
}
