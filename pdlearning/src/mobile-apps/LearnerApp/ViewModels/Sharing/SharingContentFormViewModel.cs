using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Common.TaskController;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Sharing;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.Services.Backend;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Sharing
{
    public class SharingContentFormViewModel : BasePageViewModel
    {
        private readonly ILearnerBackendService _learnerBackendService;
        private readonly IOrganizationBackendService _organizationBackendService;
        private readonly StressActionHandler _loadMoreStressHandler = new StressActionHandler();

        private readonly SingleTaskExecutionRunner _singleTaskExecutionRunner = new SingleTaskExecutionRunner(
            TimeSpan.FromSeconds(1));

        private readonly List<string> _sharingRecipientIdList = new List<string>();

        private string _searchText;
        private bool _isLoading;
        private LearnerObservableCollection<SharingSuggestionRecipientViewModel> _sharingSuggestions
            = new LearnerObservableCollection<SharingSuggestionRecipientViewModel>();

        private bool _ableToLoadMore = true;
        private int _totalCount;
        private ISharingContentFormDelegate _delegate;

        public SharingContentFormViewModel()
        {
            _organizationBackendService = CreateRestClientFor<IOrganizationBackendService>(GlobalSettings.BackendServiceOrganization);
            _learnerBackendService = CreateRestClientFor<ILearnerBackendService>(GlobalSettings.BackendServiceLearner);
        }

        public override string PageTitle => string.Empty;

        public override string RoutingName => NavigationRoutes.SharingContent;

        public ICommand SearchCommand => new Command(OnSearchCommandTriggered);

        public LearnerObservableCollection<SharingSuggestionRecipientViewModel> SharingSuggestions
        {
            get => _sharingSuggestions;
            set
            {
                _sharingSuggestions = value;
                RaisePropertyChanged(() => SharingSuggestions);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
                if (_searchText.IsNullOrEmpty())
                {
                    Device.BeginInvokeOnMainThread(async ()
                        => await StartSearching(true));
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
                RaisePropertyChanged(() => ShowNoData);
                RaisePropertyChanged(() => ShowStartSearching);
                RaisePropertyChanged(() => ShowData);
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
                RaisePropertyChanged(() => ShouldContinueLoadMore);
            }
        }

        public bool ShouldContinueLoadMore
        {
            get
            {
                return _ableToLoadMore && TotalCount > (_sharingSuggestions?.Count ?? 0);
            }
        }

        public bool ShowStartSearching => IsLoading == false && SharingSuggestions.IsNullOrEmpty() && _searchText.IsNullOrEmpty();

        public bool ShowNoData => IsLoading == false && SharingSuggestions.IsNullOrEmpty() && !_searchText.IsNullOrEmpty();

        public bool ShowData => IsLoading || SharingSuggestions.Any();

        public ICommand LoadMoreCommand => new Command(LoadMore);

        public ICommand CancelCommand => new Command(async () => await NavigationService.GoBack());

        public static NavigationParameters GetNavigationParameters(ISharingContentFormDelegate @delegate)
        {
            var param = new NavigationParameters();
            param.SetParameter("delegate", @delegate);
            return param;
        }

        public async Task<bool> Share(UserInformation userInformation)
        {
            bool success
                = await _delegate.AddShareUser(userInformation);

            if (success)
            {
                _sharingRecipientIdList.Add(userInformation.UserCxId);
            }

            return success;
        }

        protected override Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            base.InternalNavigatedTo(navigationParameters);
            _delegate = navigationParameters.GetParameter<ISharingContentFormDelegate>("delegate");
            return Task.CompletedTask;
        }

        private async void LoadMore()
        {
            await _loadMoreStressHandler.RunAsync(async () =>
            {
                await StartSearching(true);
            });
        }

        private async void OnSearchCommandTriggered(object sender)
        {
            await StartSearching(false);
        }

        private async Task StartSearching(bool loadMore)
        {
            var searchText = SearchText;
            int totalCount = 0;
            List<SharingSuggestionRecipientViewModel> sharingSuggestions = null;

            await _singleTaskExecutionRunner.RunTaskAsync(
                true,
                (token) =>
                {
                    if (!loadMore)
                    {
                        IsLoading = true;
                        _ableToLoadMore = true;
                        SharingSuggestions.Clear();
                    }

                    return Task.CompletedTask;
                },
                async (token) =>
                {
                    var suggestions = await ExecuteBackendService(() =>
                        _learnerBackendService.SearchUsers(searchText, SharingSuggestions.Count, ctoken: token));

                    if (suggestions.HasEmptyResult() || suggestions.Payload.Items.IsNullOrEmpty())
                    {
                        return;
                    }

                    totalCount = suggestions.Payload.TotalCount;
                    string[] userIds = suggestions.Payload.Items.Select(p => p.Id).ToArray();
                    var usersInfo = await ExecuteBackendService(() =>
                        _organizationBackendService.GetUserInfomation(new { UserCxIds = userIds }, ctoken: token));

                    if (usersInfo.HasEmptyResult() || usersInfo.Payload.IsNullOrEmpty())
                    {
                        return;
                    }

                    sharingSuggestions = new List<SharingSuggestionRecipientViewModel>(
                        usersInfo.Payload.Select(
                            sg => new SharingSuggestionRecipientViewModel(
                                this,
                                sg,
                                _sharingRecipientIdList.Contains(sg.UserCxId))));
                },
                (token) =>
                {
                    if (sharingSuggestions == null)
                    {
                        _ableToLoadMore = false;
                        RaisePropertyChanged(() => ShouldContinueLoadMore);
                        if (!loadMore)
                        {
                            SharingSuggestions = new LearnerObservableCollection<SharingSuggestionRecipientViewModel>();
                        }

                        IsLoading = false;
                        return Task.CompletedTask;
                    }

                    if (loadMore)
                    {
                        SharingSuggestions.AddRange(sharingSuggestions);
                    }
                    else
                    {
                        SharingSuggestions = new LearnerObservableCollection<SharingSuggestionRecipientViewModel>(sharingSuggestions);
                    }

                    IsLoading = false;
                    TotalCount = totalCount;
                    return Task.CompletedTask;
                });
        }
    }
}
