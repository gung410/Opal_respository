using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningDigitalContent : ContentView
    {
        private static INavigationService _navigationService;

        private readonly MyLearningDigitalViewModel _viewModel;
        private readonly ICommonServices _commonService;
        private readonly IDialogService _dialogService;

        private bool _isBusy = false;
        private MyLearningStatus _statusFilter;
        private LearnerObservableCollection<ItemCard> _digitalContentCollection;

        public MyLearningDigitalContent()
        {
            InitializeComponent();

            _viewModel = new MyLearningDigitalViewModel();

            _commonService = DependencyService.Resolve<ICommonServices>();
            _dialogService = DependencyService.Resolve<IDialogService>();
            _navigationService = DependencyService.Resolve<INavigationService>();

            _digitalContentCollection = new LearnerObservableCollection<ItemCard>();
            DigitalContentList.ItemsSource = _digitalContentCollection;

            _statusFilter = MyLearningStatus.InProgress;

            CourseBookmarkMessagingCenter.Subscribe(this, (sender, args) =>
            {
                UpdateBookmarkFromDetails();
            });
        }

        ~MyLearningDigitalContent()
        {
            CourseBookmarkMessagingCenter.Unsubscribe(this);
        }

        public async Task LoadMyDigitalContent(bool includeStatistic = false, bool isLoadmore = false)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            int totalFilterCount = 0;

            using (_dialogService.DisplayLoadingIndicator())
            {
                // Clear list item for re-loading data
                if (!isLoadmore)
                {
                    _digitalContentCollection.Clear();
                }

                // When statusFilter = null, the viewModel will be get all
                var cards = await _viewModel.GetMyDigitalContent(
                    skipCount: _digitalContentCollection.Count,
                    keyword: KeywordSearchEntry.Text,
                    includeStatistic: includeStatistic,
                    statusFilter: _statusFilter,
                    totalFilterCount: count =>
                    {
                        // Current total count in filter
                        totalFilterCount = count;
                        CurrentStatusFilterLbl.Text = $"{SeparateStringByUppercase.Convert(_statusFilter.ToString())} ({count})";
                    },
                    searchStatistics => OnHandleSearchStatistics(searchStatistics));

                _digitalContentCollection.AddRange(cards);

                // Enable or Disable display items
                Device.BeginInvokeOnMainThread(() =>
                {
                    DigitalContentList.IsVisible = !cards.IsNullOrEmpty();
                    EmpltListLbl.IsVisible = cards.IsNullOrEmpty();
                });
            }

            // Enable or Disable loadmore by total filter count
            DigitalContentList.RemainingItemsThreshold = _digitalContentCollection.Count < totalFilterCount ? 0 : -1;

            _isBusy = false;
        }

        private async void DigitalContentList_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            var view = sender as CollectionView;

            if (view.SelectedItem is ItemCard contentCard)
            {
                var navParams = new NavigationParameters();
                await _navigationService.NavigateToAsync<MyDigitalContentDetailsViewModel>(MyDigitalContentDetailsViewModel.GetNavigationParameters(contentCard.Id));

                view.SelectedItem = null;
            }

            _isBusy = false;
        }

        private async void BookmarkDigitalContent_Tapped(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            if ((e as TappedEventArgs).Parameter is ItemCard digitalContentItem)
            {
                digitalContentItem.BookmarkInfo = await _commonService.Bookmark(digitalContentItem.OriginalObjectId, BookmarkType.DigitalContent, digitalContentItem.BookmarkInfo == null);

                MessagingCenter.Unsubscribe<MyLearningDigitalContent, ItemCard>(this, "bookmarked-item-on-list");
                MessagingCenter.Send(this, "bookmarked-item-on-list", digitalContentItem);
            }

            _isBusy = false;
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            if (!_isBusy)
            {
                _isBusy = true;

                await LoadMyDigitalContent(includeStatistic: true, isLoadmore: false);
            }

            RefreshView.IsRefreshing = false;

            _isBusy = false;
        }

        private void UpdateBookmarkFromDetails()
        {
            RefreshView.IsRefreshing = true;
        }

        private async void OnDigitalContentSearch_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(KeywordSearchEntry.Text))
            {
                return;
            }

            _statusFilter = MyLearningStatus.InProgress;

            await LoadMyDigitalContent(includeStatistic: true, isLoadmore: false);

            Device.BeginInvokeOnMainThread(() =>
            {
                DigitalContentFilterStack.IsVisible = false;
                SearchGroupFilter.IsVisible = true;
            });
        }

        private void OnHandleSearchStatistics(List<SearchStatistics> searchStatistics)
        {
            // Create data for SearchGroupFilter control
            var filterData = new SearchGroupFilterData
            {
                Data = searchStatistics.ToDictionary(p => p.Type, p => p.TotalCount),
                Keyword = KeywordSearchEntry.Text,
                CurrentFilter = _statusFilter.ToString()
            };

            // Set data for SearchGroupFilter control
            SearchGroupFilter.FilterData = filterData;

            // Visible SearchGroupFilter control
            Device.BeginInvokeOnMainThread(() =>
            {
                SearchGroupFilter.IsVisible = true;
            });
        }

        private async void DigitalContentList_Loadmore(object sender, EventArgs e)
        {
            await LoadMyDigitalContent(isLoadmore: true);
        }

        private async void SearchGroupFilter_OnFilter(object sender, Dictionary<string, int> e)
        {
            Enum.TryParse(e.FirstOrDefault().Key, out MyLearningStatus myStatus);
            _statusFilter = myStatus;

            await LoadMyDigitalContent(includeStatistic: false, isLoadmore: false);
        }

        private async void SearchGroupFilter_OnBack(object sender, EventArgs e)
        {
            // Remove search keyword
            KeywordSearchEntry.Text = string.Empty;

            // Get all item type
            _statusFilter = MyLearningStatus.InProgress;

            // Reload data
            await LoadMyDigitalContent();

            Device.BeginInvokeOnMainThread(() =>
            {
                SearchGroupFilter.IsVisible = false;
                DigitalContentFilterStack.IsVisible = true;
            });
        }

        private void KeywordSearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                return;
            }

            SearchGroupFilter_OnBack(this, null);
        }

        private void DigitalContentStatusSelected(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var items = new Dictionary<string, string>
                {
                    { MyLearningStatus.InProgress.ToString(), null },
                    { MyLearningStatus.Completed.ToString(), null }
                };

                _dialogService.ShowDropDownSelectionPopup(items, isFullScreen: false, onSelected: async currentMyLearningGroupSelected =>
                {
                    if (!string.IsNullOrEmpty(currentMyLearningGroupSelected))
                    {
                        Enum.TryParse(currentMyLearningGroupSelected, out MyLearningStatus myStatus);

                        _statusFilter = myStatus;
                        await LoadMyDigitalContent(includeStatistic: false, isLoadmore: false);
                    }
                });
            });
        }
    }
}
