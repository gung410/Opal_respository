using System.Threading.Tasks;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class LearningOpportunityUpcomingSessionCardViewModel : BaseViewModel
    {
        public async Task OpenCheckInPopup()
        {
            await NavigationService.NavigateToAsync<CheckInViewModel>();
        }

        public async Task OpenCannotParticipatePopup(string sessionId)
        {
            var parameters = new NavigationParameters();
            parameters.SetParameter("sessionId", sessionId);
            await NavigationService.NavigateToAsync<CannotParticipateViewModel>(parameters);
        }
    }
}
