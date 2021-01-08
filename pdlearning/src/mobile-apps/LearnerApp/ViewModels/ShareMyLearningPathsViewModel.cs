using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.MyLearning;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.ViewModels
{
    public class ShareMyLearningPathsViewModel : BasePageViewModel, INavigationAware
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;

        private LearningPath _learningPath;
        private LearningPathSharing _shareLearningPath;
        private string _textSearch;
        private string _totalUserShared;
        private bool _isVisibleListSharedUser;

        public ShareMyLearningPathsViewModel()
        {
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);

            IsVisibleListSharedUser = false;
        }

        public ObservableCollection<UserInformation> UserCollection { get; set; }

        public ICommand CancelCommand => new Command(OnCancel);

        public ICommand ConfirmCommand => new Command(async () => await OnConfirm());

        public ICommand SearchCommand => new Command(OnSearch);

        public string TextSearch
        {
            get
            {
                return _textSearch;
            }

            set
            {
                _textSearch = value;
                RaisePropertyChanged(() => TextSearch);
            }
        }

        public string TotalUserShared
        {
            get
            {
                return _totalUserShared;
            }

            set
            {
                _totalUserShared = value;
                RaisePropertyChanged(() => TotalUserShared);
            }
        }

        public bool IsVisibleListSharedUser
        {
            get
            {
                return _isVisibleListSharedUser;
            }

            set
            {
                _isVisibleListSharedUser = value;
                RaisePropertyChanged(() => IsVisibleListSharedUser);
            }
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.MyLearningPathShare;

        public void OnRemoveShareUser(UserInformation user)
        {
            DialogService.ConfirmAsync("Do you want to stop sharing the learning path to this user?", "No", "Yes", onConfirmed: (confirmed) =>
            {
                if (confirmed)
                {
                    UserCollection.Remove(user);

                    SetTotalCount();

                    CrossToastPopUp.Current.ShowToastSuccess("Stop sharing successfully");

                    IsVisibleListSharedUser = UserCollection != null;
                }
            });
        }

        protected override async Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            _learningPath = navigationParameters.GetParameter<LearningPath>("learning-paths-item");

            await GetSharingInfomation();
        }

        private void SetTotalCount()
        {
            TotalUserShared = UserCollection.Count == 1 ? $"{UserCollection.Count} USER SELECTED" : $"{UserCollection.Count} USERS SELECTED";
        }

        private async Task GetSharingInfomation()
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var result = await ExecuteBackendService(() => _learnerBackendService.GetLearningPathsSharedList(_learningPath.Id));

                if (result.HasEmptyResult() || result.Payload.Users.IsNullOrEmpty())
                {
                    return;
                }

                _shareLearningPath = result.Payload;

                var userInfo = await GetUsersInfor(result.Payload.Users.Select(p => p.UserId).ToArray());

                if (!userInfo.IsNullOrEmpty())
                {
                    UserCollection = new ObservableCollection<UserInformation>(userInfo);
                    RaisePropertyChanged(() => UserCollection);

                    IsVisibleListSharedUser = true;

                    SetTotalCount();
                }
            }
        }

        private async Task OnConfirm()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            var usersSharedIds = new List<LearningPathShareUser>();
            if (!UserCollection.IsNullOrEmpty())
            {
                foreach (var item in UserCollection)
                {
                    usersSharedIds.Add(new LearningPathShareUser
                    {
                        UserId = item.UserCxId
                    });
                }
            }

            if (_shareLearningPath == null)
            {
                await CreareLearningPath(usersSharedIds);
            }
            else
            {
                await UpdateLeaningPath(usersSharedIds);
            }

            await NavigationService.GoBack();

            CrossToastPopUp.Current.ShowToastSuccess("Shared learning path successfully");

            IsBusy = false;
        }

        private async Task CreareLearningPath(List<LearningPathShareUser> usersSharedIds)
        {
            var param = new
            {
                ItemId = _learningPath.Id,
                ItemType = SharingType.LearningPath,
                UsersShared = usersSharedIds
            };

            await ExecuteBackendService(() => _learnerBackendService.CreateShareLearningPath(param));
        }

        private async Task UpdateLeaningPath(List<LearningPathShareUser> usersSharedIds)
        {
            var param = new
            {
                Id = _shareLearningPath.Id,
                ItemId = _shareLearningPath.ItemId,
                ItemType = SharingType.LearningPath,
                UsersShared = usersSharedIds
            };

            await ExecuteBackendService(() => _learnerBackendService.UpdateSharedLearningPath(param));
        }

        private async void OnCancel(object obj)
        {
            await NavigationService.GoBack();
        }

        private void OnSearch(object obj)
        {
            if (string.IsNullOrEmpty(TextSearch))
            {
                return;
            }

            DialogService.ShowLearningPathsSelectUser(UserCollection, TextSearch, async (arg) =>
            {
                var userInfo = await GetUsersInfor(arg.Select(p => p.UserCxId).ToArray());

                if (userInfo != null)
                {
                    UserCollection = new ObservableCollection<UserInformation>(userInfo);
                }

                RaisePropertyChanged(() => UserCollection);

                IsVisibleListSharedUser = true;

                SetTotalCount();
            });
        }

        private async Task<List<UserInformation>> GetUsersInfor(string[] userIds)
        {
            var usersInfo = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(new { UserCxIds = userIds }));

            if (usersInfo.HasEmptyResult())
            {
                return null;
            }

            return usersInfo.Payload;
        }
    }
}
