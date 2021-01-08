using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Common.MessagingCenterManager;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningDigitalContentBookmarked : ContentView
    {
        public EventHandler BackEventHandler;

        private readonly LearnerObservableCollection<ItemCard> _source;
        private readonly ICommonServices _commonService;
        private readonly INavigationService _navigationService;

        private bool _isBusy;
        private int _totalCount = 0;

        public MyLearningDigitalContentBookmarked()
        {
            InitializeComponent();

            _commonService = DependencyService.Resolve<ICommonServices>();
            _navigationService = DependencyService.Resolve<INavigationService>();
            _source = new LearnerObservableCollection<ItemCard>();
            DataList.ItemsSource = _source;

            MyDigitalContentBookmarkMessagingCenter.Subscribe(this, (sender, arg) =>
            {
                if (_source.IsNullOrEmpty())
                {
                    return;
                }

                var item = _source.FirstOrDefault(x => x.Id == arg.DigitalContentId);
                if (item == null)
                {
                    return;
                }

                if (arg.IsBookmarked == false)
                {
                    _source.Remove(item);
                }
                else
                {
                    _source.Insert(0, item);
                }
            });
        }

        ~MyLearningDigitalContentBookmarked()
        {
            CourseBookmarkMessagingCenter.Unsubscribe(this);
        }

        public async Task LoadMyDigitalContentBookmarked(bool isLoadMore = false)
        {
            if (!isLoadMore)
            {
                _source.Clear();
            }

            List<MyDigitalContentSummary> digitalContentBookmarked = await _commonService.GetMyDigitalContentBookmarked(_source.Count, count =>
            {
                _totalCount = count;
            });

            if (!digitalContentBookmarked.IsNullOrEmpty())
            {
                var digitalContentCards = await _commonService.CreateDigitalContentCourseCard(digitalContentBookmarked.Select(p => p.DigitalContentId).ToArray());

                if (!digitalContentCards.IsNullOrEmpty())
                {
                    _source.AddRange(digitalContentCards);
                }
            }
        }

        private void BackButton_Tapped(object sender, EventArgs e)
        {
            BackEventHandler?.Invoke(this, null);
        }

        private async void UnBookmark_Tapped(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            var courseCard = (e as TappedEventArgs).Parameter as ItemCard;

            if (courseCard != null)
            {
                _source.Remove(courseCard);

                courseCard.BookmarkInfo = await _commonService.Bookmark(courseCard.OriginalObjectId, BookmarkType.DigitalContent, courseCard.BookmarkInfo == null);

                MessagingCenter.Unsubscribe<MyLearningDigitalContentBookmarked, ItemCard>(this, "bookmarked-on-list");
                MessagingCenter.Send(this, "bookmarked-on-list", courseCard);
            }

            _isBusy = false;
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            _source.Clear();

            await LoadMyDigitalContentBookmarked();

            RefreshView.IsRefreshing = false;
        }

        private async void DataList_LoadMore(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            var view = sender as CollectionView;

            if (view.ItemsSource != null)
            {
                int currentCount = (view.ItemsSource as ObservableCollection<ItemCard>).Count;

                if (currentCount < _totalCount && e.LastVisibleItemIndex == currentCount - 1)
                {
                    await LoadMyDigitalContentBookmarked(true);
                }
            }

            _isBusy = false;
        }

        private async void DataList_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            var view = sender as CollectionView;
            var contentCard = (ItemCard)view.SelectedItem;

            await _navigationService.NavigateToAsync<MyDigitalContentDetailsViewModel>(
                MyDigitalContentDetailsViewModel.GetNavigationParameters(contentCard.Id));

            view.SelectedItem = null;
            _isBusy = false;
        }
    }
}
