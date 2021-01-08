using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class WidgetSettingViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ILearnerBackendService _learnerBackendService;

        private bool _showMyLearning;
        private bool _showReccommendedForYou;
        private bool _showReccommendedByYourOrg;
        private bool _showBookmarks;
        private bool _showNewsFeed;
        private bool _showOutstanding;
        private bool _showSharedByOtherUsers;
        private bool _showCalendar;

        private bool _enableSubmit;

        public WidgetSettingViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
        }

        public ICommand UpdateSettingCommand => new Command(async () => await UpdateSetting());

        public ICommand CancelCommand => new Command(async () => await Cancel());

        public ICommand ToggledChangedCommand => new Command(() =>
        {
            if (IsBusy)
            {
                return;
            }

            UpdateSubmitButttonState();
        });

        public bool ShowCalendar
        {
            get
            {
                return _showCalendar;
            }

            set
            {
                _showCalendar = value;
                RaisePropertyChanged(() => ShowCalendar);
            }
        }

        public bool ShowOutstanding
        {
            get
            {
                return _showOutstanding;
            }

            set
            {
                _showOutstanding = value;
                RaisePropertyChanged(() => ShowOutstanding);
            }
        }

        public bool ShowMyLearning
        {
            get
            {
                return _showMyLearning;
            }

            set
            {
                _showMyLearning = value;
                RaisePropertyChanged(() => ShowMyLearning);
            }
        }

        public bool ShowNewsFeed
        {
            get
            {
                return _showNewsFeed;
            }

            set
            {
                _showNewsFeed = value;
                RaisePropertyChanged(() => ShowNewsFeed);
            }
        }

        public bool ShowReccommendedForYou
        {
            get
            {
                return _showReccommendedForYou;
            }

            set
            {
                _showReccommendedForYou = value;
                RaisePropertyChanged(() => ShowReccommendedForYou);
            }
        }

        public bool ShowReccommendedByYourOrg
        {
            get
            {
                return _showReccommendedByYourOrg;
            }

            set
            {
                _showReccommendedByYourOrg = value;
                RaisePropertyChanged(() => ShowReccommendedByYourOrg);
            }
        }

        public bool ShowBookmarks
        {
            get
            {
                return _showBookmarks;
            }

            set
            {
                _showBookmarks = value;
                RaisePropertyChanged(() => ShowBookmarks);
            }
        }

        public bool ShowSharedByOtherUsers
        {
            get
            {
                return _showSharedByOtherUsers;
            }

            set
            {
                _showSharedByOtherUsers = value;
                RaisePropertyChanged(() => ShowSharedByOtherUsers);
            }
        }

        public bool EnableSubmit
        {
            get
            {
                return _enableSubmit;
            }

            set
            {
                _enableSubmit = value;
                RaisePropertyChanged(() => EnableSubmit);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.Settings;

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                // Set IsBusy here for ignore toggle change event when init this widget setting.
                IsBusy = true;

                var reference = await ExecuteBackendService(() => _learnerBackendService.GetUserPreference(new string[] { }));
                if (!reference.HasEmptyResult() && reference.Payload.Any())
                {
                    var settings = reference.Payload.ToDictionary(p => p.Key, p => p.Value);

                    ShowNewsFeed = settings[WidgetKeys.HomeNewsfeedShow];
                    ShowOutstanding = settings[WidgetKeys.HomeOutstandingShow];
                    ShowMyLearning = settings[WidgetKeys.HomeMyLearningShow];
                    ShowReccommendedForYou = settings[WidgetKeys.HomeRecommendForYouShow];
                    ShowReccommendedByYourOrg = settings[WidgetKeys.HomeRecommendForOrgShow];
                    ShowBookmarks = settings[WidgetKeys.HomeBookmarkShow];
                    ShowSharedByOtherUsers = settings[WidgetKeys.HomeSharedByOtherUsersShow];
                    ShowCalendar = settings[WidgetKeys.HomeCalendarShow];
                }

                UpdateSubmitButttonState();

                IsBusy = false;
            }
        }

        private void UpdateSubmitButttonState()
        {
            EnableSubmit = ShowMyLearning ||
                ShowReccommendedForYou ||
                ShowReccommendedByYourOrg ||
                ShowBookmarks ||
                ShowNewsFeed ||
                ShowOutstanding ||
                ShowSharedByOtherUsers ||
                ShowCalendar;
        }

        private async Task UpdateSetting()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var updatedData = new[]
                {
                    new { Key = WidgetKeys.HomeNewsfeedShow, ValueString = ShowNewsFeed.ToString() },
                    new { Key = WidgetKeys.HomeOutstandingShow, ValueString = ShowOutstanding.ToString() },
                    new { Key = WidgetKeys.HomeMyLearningShow, ValueString = ShowMyLearning.ToString() },
                    new { Key = WidgetKeys.HomeRecommendForYouShow, ValueString = ShowReccommendedForYou.ToString() },
                    new { Key = WidgetKeys.HomeRecommendForOrgShow, ValueString = ShowReccommendedByYourOrg.ToString() },
                    new { Key = WidgetKeys.HomeBookmarkShow, ValueString = ShowBookmarks.ToString() },
                    new { Key = WidgetKeys.HomeSharedByOtherUsersShow, ValueString = ShowSharedByOtherUsers.ToString() },
                    new { Key = WidgetKeys.HomeCalendarShow, ValueString = ShowCalendar.ToString() }
                };

                await ExecuteBackendService(() => _learnerBackendService.UpdateUserPreference(param: updatedData));

                MessagingCenter.Unsubscribe<WidgetSettingViewModel>(this, "home-widget-configs");
                MessagingCenter.Send(this, "home-widget-configs");

                await NavigationService.GoBack();
            }
        }

        private async Task Cancel()
        {
            await NavigationService.GoBack();
        }
    }
}
