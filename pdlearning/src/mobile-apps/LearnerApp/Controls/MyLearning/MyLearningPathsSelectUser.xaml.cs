using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.ViewModels.MyLearning;
using Plugin.Toast;
using Rg.Plugins.Popup.Services;
using Telerik.XamarinForms.DataControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningPathsSelectUser
    {
        public EventHandler<List<UserInformation>> OnUsersSelectedEventHandler;

        private MyLearningPathsSelectUserViewModel _viewModel;
        private string _searchText;
        private StressActionHandler _loadMoreActionHandler = new StressActionHandler();

        public MyLearningPathsSelectUser(IEnumerable<UserInformation> userCollection, string searchText)
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningPathsSelectUserViewModel(userCollection);

            _searchText = searchText;

            Device.BeginInvokeOnMainThread(() =>
            {
                SearchEntry.Text = _searchText;
                ResultSearchLbl.Text = string.IsNullOrEmpty(_searchText) ? "Results for" : $"Results for <b>{_searchText}</b>";
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.SearchUserSharing(_searchText);
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAsync();
            }
        }

        private async void OnSearch(object sender, EventArgs e)
        {
            _viewModel.SkipCount = 0;

            Device.BeginInvokeOnMainThread(() =>
            {
                ResultSearchLbl.Text = string.IsNullOrEmpty(SearchEntry.Text) ? "Results for" : $"Results for <b>{SearchEntry.Text}</b>";
            });

            await _viewModel.SearchUserSharing(SearchEntry.Text);
        }

        private void OnDone(object sender, EventArgs e)
        {
            if (_viewModel.ListItemSelected.IsNullOrEmpty())
            {
                CrossToastPopUp.Current.ShowToastError("Please select user");

                return;
            }

            OnUsersSelectedEventHandler?.Invoke(this, _viewModel.ListItemSelected);

            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAsync();
            }
        }

        private void RadListView_SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var view = sender as RadListView;

            if (!(view.SelectedItem is UserInformation item))
            {
                return;
            }

            _viewModel.OnItemSelected(item);
        }

        private async void CourseSourse_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            await _loadMoreActionHandler.RunAsync(async () =>
            {
                var view = sender as CollectionView;

                if (view.ItemsSource == null)
                {
                    return;
                }

                int currentCount = (view.ItemsSource as ObservableCollection<UserInformation>).Count;

                if (currentCount < _viewModel.TotalCount && e.LastVisibleItemIndex == currentCount - 1)
                {
                    _viewModel.SkipCount++;
                    await _viewModel.SearchUserSharing(SearchEntry.Text);
                }
            });
        }

        private void CourseSourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = sender as CollectionView;
            var courseCard = view.SelectedItem as UserInformation;

            if (courseCard != null)
            {
                _viewModel.OnItemSelected(courseCard);
            }
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            await _viewModel.OnRefreshing(SearchEntry.Text);
        }
    }
}
