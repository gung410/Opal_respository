using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class CheckInViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ICourseBackendService _courseBackendService;

        private string _sessionCode;

        public CheckInViewModel()
        {
            _courseBackendService = CreateRestClientFor<ICourseBackendService>(GlobalSettings.BackendServiceCourse);
        }

        public string SessionCode
        {
            get
            {
                return _sessionCode;
            }

            set
            {
                _sessionCode = value;
                RaisePropertyChanged(() => SessionCode);
            }
        }

        public ICommand CancelCommand => new Command(async () => await CancelCheckin());

        public ICommand ConfirmCommand => new Command(async () => await ConfirmCheckin());

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.CheckIn;

        private async Task CancelCheckin()
        {
            await NavigationService.GoBack();
        }

        private async Task ConfirmCheckin()
        {
            var sessionObj = new { SessionCode };

            var attendanceTrackingResponse = await ExecuteBackendService(() => _courseBackendService.TakeAttendance(sessionObj));

            if (attendanceTrackingResponse.HasEmptyResult())
            {
                return;
            }

            MessagingCenter.Unsubscribe<CheckInViewModel>(this, "on-back-to-course-details");
            MessagingCenter.Send(this, "on-back-to-course-details");

            await NavigationService.GoBack();
        }
    }
}
