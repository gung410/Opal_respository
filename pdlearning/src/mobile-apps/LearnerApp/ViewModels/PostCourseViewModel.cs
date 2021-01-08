using System.Threading.Tasks;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class PostCourseViewModel : BasePageViewModel
    {
        private readonly ICourseBackendService _courseBackendService;
        private string _formId;
        private string _resourceId;
        private WebViewContentType _webviewContentType;
        private string _webviewSource;
        private string _registrationId;

        public PostCourseViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
        }

        ~PostCourseViewModel()
        {
            MessagingCenter.Unsubscribe<PostCourseViewModel>(this, "on-back-to-course-details-from-post-course");
        }

        public string FormId
        {
            get
            {
                return _formId;
            }

            set
            {
                _formId = value;
                RaisePropertyChanged(() => FormId);
            }
        }

        public string ResourceId
        {
            get
            {
                return _resourceId;
            }

            set
            {
                _resourceId = value;
                RaisePropertyChanged(() => ResourceId);
            }
        }

        public WebViewContentType WebviewContentType
        {
            get
            {
                return _webviewContentType;
            }

            set
            {
                _webviewContentType = value;
                RaisePropertyChanged(() => WebviewContentType);
            }
        }

        public string WebviewSource
        {
            get
            {
                return _webviewSource;
            }

            set
            {
                _webviewSource = value;
                RaisePropertyChanged(() => WebviewSource);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CourseDetailsPost;

        public async Task CompleteCourse()
        {
            await ExecuteBackendService(() => _courseBackendService.CompletePostEvaluation(_registrationId));

            MessagingCenter.Send(this, "on-back-to-course-details-from-post-course");

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;
                await NavigationService.GoBack();
                IsBusy = false;
            });
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                FormId = navigationParameters.GetParameter<string>("formId");
                ResourceId = navigationParameters.GetParameter<string>("resourceId");
                _registrationId = navigationParameters.GetParameter<string>("registrationId");
                WebviewContentType = WebViewContentType.Internet;
                WebviewSource = GlobalSettings.WebViewUrlQuizPlayer;
                return Task.CompletedTask;
            }
        }
    }
}
