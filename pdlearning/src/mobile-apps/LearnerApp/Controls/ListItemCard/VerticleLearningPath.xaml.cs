using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Resources.Texts;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticleLearningPath : ContentView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ObservableCollection<LearningPath>), typeof(VerticleLearningPath), null, propertyChanged: OnSourceChanged);
        public static readonly BindableProperty TotalCountProperty = BindableProperty.Create(nameof(TotalCount), typeof(int), typeof(VerticleLearningPath), null, propertyChanged: OnTotalCountChanged);
        public static readonly BindableProperty OnLoadmoreCommandProperty = BindableProperty.Create(nameof(OnLoadmoreCommand), typeof(ICommand), typeof(VerticleLearningPath), null);
        public static readonly BindableProperty OnRefreshCommandProperty = BindableProperty.Create(nameof(OnRefreshCommand), typeof(ICommand), typeof(VerticleLearningPath), null);
        public static readonly BindableProperty MyLearningPathTypeProperty = BindableProperty.Create(nameof(MyLearningPathType), typeof(LearningPathType), typeof(VerticleLearningPath), null);
        public static readonly BindableProperty IsRemoveWhenUnBookmarkProperty = BindableProperty.Create(nameof(IsRemoveWhenUnBookmark), typeof(bool), typeof(VerticleLearningPath), null);

        private static INavigationService _navigationService;
        private static bool _isBusy = false;
        private readonly ICommonServices _commonService;

        public VerticleLearningPath()
        {
            InitializeComponent();

            _navigationService = DependencyService.Resolve<INavigationService>();
            _commonService = DependencyService.Resolve<ICommonServices>();
        }

        public bool IsRemoveWhenUnBookmark
        {
            get { return (bool)GetValue(IsRemoveWhenUnBookmarkProperty); }
            set { SetValue(IsRemoveWhenUnBookmarkProperty, value); }
        }

        public LearningPathType MyLearningPathType
        {
            get { return (LearningPathType)GetValue(MyLearningPathTypeProperty); }
            set { SetValue(MyLearningPathTypeProperty, value); }
        }

        public ICommand OnLoadmoreCommand
        {
            get { return (ICommand)GetValue(OnLoadmoreCommandProperty); }
            set { SetValue(OnLoadmoreCommandProperty, value); }
        }

        public ICommand OnRefreshCommand
        {
            get { return (ICommand)GetValue(OnRefreshCommandProperty); }
            set { SetValue(OnRefreshCommandProperty, value); }
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        public ObservableCollection<LearningPath> Source
        {
            get { return (ObservableCollection<LearningPath>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (VerticleLearningPath)bindable;

            if (newValue is ObservableCollection<LearningPath> items && items.Any())
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    bool isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
                    if (!isVisibleBookmark)
                    {
                        foreach (var item in items)
                        {
                            item.IsVisibleBookmark = false;
                        }
                    }

                    view.LearningPathsCollectionView.ItemsSource = items;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    view.EmptyViewMessage.Text = TextsResource.NOTHING_HERE_YET;
                });
            }
        }

        private static void OnTotalCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (VerticleLearningPath)bindable;

            int totalCount = (int)newValue;

            if (totalCount == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    view.LearningPathsCollectionView.ItemsSource = null;
                    view.EmptyViewMessage.Text = TextsResource.NOTHING_HERE_YET;
                });
            }
        }

        private async void CourseSourse_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            var view = sender as CollectionView;

            if (view.SelectedItem is LearningPath learningPath)
            {
                var navParams = new NavigationParameters();
                navParams.SetParameter("learning-path-id", learningPath.Id);
                navParams.SetParameter("is-my-own-learning-path", MyLearningPathType);

                await _navigationService.NavigateToAsync<MyLearningPathsDetailsViewModel>(navParams);

                view.SelectedItem = null;
            }

            _isBusy = false;
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            if (OnRefreshCommand != null)
            {
                OnRefreshCommand.Execute(null);
            }

            LearningPathsRefreshView.IsRefreshing = false;
        }

        private void CourseSourse_LoadMore(object sender, ItemsViewScrolledEventArgs e)
        {
            var view = sender as CollectionView;

            if (view.ItemsSource == null || OnLoadmoreCommand == null)
            {
                return;
            }

            int currentCount = (view.ItemsSource as ObservableCollection<LearningPath>).Count;

            if (currentCount < TotalCount && e.LastVisibleItemIndex == currentCount - 1)
            {
                OnLoadmoreCommand.Execute(null);
            }
        }

        private async void OnBookmark_Tapped(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            LearningPath learningPathsBookmark = (e as TappedEventArgs).Parameter as LearningPath;

            learningPathsBookmark.BookmarkInfo = await _commonService.Bookmark(learningPathsBookmark.Id, MyLearningPathType == LearningPathType.RecommendationLearningPath ? BookmarkType.LearningPathLMM : BookmarkType.LearningPath, learningPathsBookmark.BookmarkInfo == null);

            if (IsRemoveWhenUnBookmark && learningPathsBookmark.BookmarkInfo == null)
            {
                Source.Remove(learningPathsBookmark);
                TotalCount--;
                if (Source.Count == 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        EmptyViewMessage.Text = TextsResource.NOTHING_HERE_YET;
                    });
                }
            }

            _isBusy = false;
        }
    }
}
