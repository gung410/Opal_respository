using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using LearnerApp.Views;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class MainTopbarViewModel : BaseViewModel, INavigationAware
    {
        private readonly IDialogService _dialogService;
        private readonly ICommunicationBackendService _communicationBackendService;

        private string _avatar;
        private int _totalNewNotification;

        public MainTopbarViewModel()
        {
            _dialogService = DependencyService.Resolve<IDialogService>();

            _communicationBackendService = CreateRestClientFor<ICommunicationBackendService>(GlobalSettings.BackendServiceCommunication);

            MessagingCenter.Subscribe<NotificationsView>(this, "reload-new-notification", async (sender) =>
            {
                TotalNewNotification = await GetTotalNewNotification();
            });
        }

        /// <summary>
        /// We have multiple top bars, but they all behave the same and getting the same data.
        /// That's why we have a singleton for this.
        /// </summary>
        public static MainTopbarViewModel Instance { get; } = new MainTopbarViewModel();

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

        public int TotalNewNotification
        {
            get
            {
                return _totalNewNotification;
            }

            set
            {
                _totalNewNotification = value;
                RaisePropertyChanged(() => TotalNewNotification);
            }
        }

        public ICommand NavigateToMyProfileCommand => new Command(async () => await OnNavigationMyProfile());

        public override void Dispose()
        {
            base.Dispose();
            MessagingCenter.Unsubscribe<NotificationsView>(this, "reload-new-notification");
        }

        public async Task OnNavigatedTo(NavigationParameters navigationParameters)
        {
            GetUserAvatar();

            TotalNewNotification = await GetTotalNewNotification();
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

        private async Task<int> GetTotalNewNotification()
        {
            int totalNewNotification = 0;

            var user = Application.Current.Properties.GetAccountProperties()?.User;

            if (user == null)
            {
                return totalNewNotification;
            }

            string userId = user.Sub;

            var notificationResult = await ExecuteBackendService(() => _communicationBackendService.GetNotificationHistory(userId));

            if (!notificationResult.IsError)
            {
                totalNewNotification = notificationResult.Payload.TotalUnreadCount;
            }

            return totalNewNotification;
        }

        private async Task OnNavigationMyProfile()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                int totalNewNotification = await GetTotalNewNotification();

                var items = new Dictionary<string, string>
                {
                    { "Settings", null },
                    { "Notifications", null },
                    { "Check-in", null },
                    { "Sign Out", null }
                };

                await _dialogService.ShowDropDownSelectionPopup(items, totalNewNotification: totalNewNotification, isFullScreen: true, isSeparateStringByUppercase: false, onSelected: async myProfileNavigated =>
                {
                    if (!string.IsNullOrEmpty(myProfileNavigated))
                    {
                        switch (myProfileNavigated)
                        {
                            case "Settings":
                                await NavigationService.NavigateToAsync<MyProfileViewModel>();
                                break;
                            case "Notifications":
                                await NavigationService.NavigateToAsync<NotificationsViewModel>();
                                break;
                            case "Check-in":
                                await NavigationService.NavigateToAsync<CheckInViewModel>();
                                break;
                            case "Sign Out":
                                await SignOut();
                                break;
                        }
                    }
                });
            }
        }

        private async Task SignOut()
        {
            using (_dialogService.DisplayLoadingIndicator())
            {
                await this.DialogService.ConfirmAsync("Are you sure you want to logout?", confirmedTextBtn: "Logout", onConfirmed: async (confirmed) =>
                {
                    if (!confirmed)
                    {
                        return;
                    }

                    await this.NavigationService.NavigateToAsync<LogoutViewModel>();
                });
            }
        }
    }
}
