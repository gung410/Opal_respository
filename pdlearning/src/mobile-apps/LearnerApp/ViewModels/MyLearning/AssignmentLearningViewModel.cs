using System.Threading.Tasks;
using LearnerApp.Models.Course;
using LearnerApp.Models.MyLearning.DigitalContentPlayer;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class AssignmentLearningViewModel : BasePageViewModel
    {
        private string _pageTitle;
        private AssignmentDetail _assignmentDetail;
        private AssignmentDigitalContentPlayerData _digitalContentPlayerData;

        public override string PageTitle => _pageTitle;

        public override string RoutingName => NavigationRoutes.AssignmentContentPlayer;

        public AssignmentDigitalContentPlayerData DigitalContentPlayerData
        {
            get => _digitalContentPlayerData;
            set
            {
                _digitalContentPlayerData = value;
                RaisePropertyChanged(() => DigitalContentPlayerData);
            }
        }

        public static NavigationParameters GetNavigationParameters(AssignmentDetail assignmentDetail)
        {
            var param = new NavigationParameters();
            param.SetParameter("assignment-detail", assignmentDetail);
            return param;
        }

        public void CloseAssignmentLearning()
        {
            MessagingCenter.Send(this, "on-close-assignment-player");
            NavigationService.GoBack();
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            _assignmentDetail = navigationParameters.GetParameter<AssignmentDetail>("assignment-detail");
            _pageTitle = _assignmentDetail.Title;
            RaisePropertyChanged(() => PageTitle);
            DigitalContentPlayerData = new AssignmentDigitalContentPlayerData(
                (await IdentityService.GetAccountPropertiesAsync()).AccessToken,
                _assignmentDetail);

            DigitalContentPlayerData.OnAssignmentBack += CloseAssignmentLearning;
            DigitalContentPlayerData.OnAssignmentSubmitted += CloseAssignmentLearning;
        }
    }
}
