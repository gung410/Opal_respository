using System.Threading.Tasks;
using LearnerApp.Models.MyLearning.DigitalContentPlayer;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.ViewModels
{
    public class StandAloneFormViewModel : BasePageViewModel, INavigationAware
    {
        private StandaloneDigitalContentPlayerData _standAlonePlayerData;

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.StandaloneForm;

        public StandaloneDigitalContentPlayerData StandAlonePlayerData
        {
            get
            {
                return _standAlonePlayerData;
            }

            set
            {
                _standAlonePlayerData = value;
                RaisePropertyChanged(() => StandAlonePlayerData);
            }
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            await base.InternalNavigatedTo(navigationParameters);
            string standALoneFormId = navigationParameters.GetParameter<string>("form-id");

            if (!string.IsNullOrEmpty(standALoneFormId))
            {
                string accessToken = (await IdentityService.GetAccountPropertiesAsync())?.AccessToken;

                StandAlonePlayerData = new StandaloneDigitalContentPlayerData(
                    accessToken,
                    standALoneFormId);

                StandAlonePlayerData.OnQuizFinished += async s =>
                {
                    await NavigationService.GoBack();
                };
            }
        }
    }
}
