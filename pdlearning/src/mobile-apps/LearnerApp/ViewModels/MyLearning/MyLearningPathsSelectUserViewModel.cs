using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Backend;
using LearnerApp.ViewModels.Base;
using Plugin.Toast;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.MyLearning
{
    public class MyLearningPathsSelectUserViewModel : BaseViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;

        private bool _isRefreshing;
        private string _emptyListMessage;

        public MyLearningPathsSelectUserViewModel(IEnumerable<UserInformation> userCollection)
        {
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);

            ListItemSelected = new List<UserInformation>();
            UserCollection = new LearnerObservableCollection<UserInformation>();
            SkipCount = 0;
            EmptyListMessage = "Loading...";

            if (!userCollection.IsNullOrEmpty())
            {
                ListItemSelected.AddRange(userCollection);
            }
        }

        public ICommand DismissSearchCommand => new Command(OnCloseSearchView);

        public LearnerObservableCollection<UserInformation> UserCollection { get; set; }

        public List<UserInformation> ListItemSelected { get; set; }

        public string EmptyListMessage
        {
            get
            {
                return _emptyListMessage;
            }

            set
            {
                _emptyListMessage = value;
                RaisePropertyChanged(() => EmptyListMessage);
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        public int TotalCount { get; set; }

        public int SkipCount { get; set; }

        public async Task SearchUserSharing(string textSearch)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var result = await ExecuteBackendService(() => _learnerBackendService.SearchUsers(textSearch, skipCount: SkipCount * GlobalSettings.MaxResultPerPage));

                if (result.HasEmptyResult() || result.Payload.Items.IsNullOrEmpty())
                {
                    EmptyListMessage = TextsResource.NOTHING_HERE_YET;
                    return;
                }

                TotalCount = result.Payload.TotalCount;

                string[] userIds = result.Payload.Items.Select(p => p.Id).ToArray();
                var usersInfo = await ExecuteBackendService(() => _organizationBackendService.GetUserInfomation(new { UserCxIds = userIds }));

                if (usersInfo.HasEmptyResult() || usersInfo.Payload.IsNullOrEmpty())
                {
                    return;
                }

                if (SkipCount == 0)
                {
                    UserCollection = new LearnerObservableCollection<UserInformation>(usersInfo.Payload);
                }
                else
                {
                    UserCollection.AddRange(usersInfo.Payload);
                }

                RaisePropertyChanged(() => UserCollection);
            }
        }

        public void OnItemSelected(UserInformation item)
        {
            if (ListItemSelected.FirstOrDefault(p => p.UserCxId == item.UserCxId) == null)
            {
                ListItemSelected.Add(item);

                CrossToastPopUp.Current.ShowToastSuccess("User added successfully");
            }
            else
            {
                CrossToastPopUp.Current.ShowToastWarning("User already added");
            }

            UserCollection.Remove(item);
            RaisePropertyChanged(() => UserCollection);

            if (UserCollection.IsNullOrEmpty())
            {
                EmptyListMessage = TextsResource.NOTHING_HERE_YET;
            }
        }

        public async Task OnRefreshing(string searchText)
        {
            SkipCount = 0;

            await SearchUserSharing(searchText);

            IsRefreshing = false;
        }

        private void OnCloseSearchView(object obj)
        {
            SkipCount = 0;

            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAsync();
            }
        }
    }
}
