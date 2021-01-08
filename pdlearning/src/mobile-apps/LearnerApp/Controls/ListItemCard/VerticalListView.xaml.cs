using System;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using LearnerApp.Resources.Texts;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticalListView : ContentView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(LearnerObservableCollection<ItemCard>), typeof(VerticalListView), null, propertyChanged: OnSourceChanged);
        public static readonly BindableProperty TotalCountProperty = BindableProperty.Create(nameof(TotalCount), typeof(int), typeof(VerticalListView), -1, propertyChanged: OnTotalCountChanged);
        public static readonly BindableProperty IsRemoveWhenUnBookmarkProperty = BindableProperty.Create(nameof(IsRemoveWhenUnBookmark), typeof(bool), typeof(VerticalListView), null);
        public static readonly BindableProperty OnLoadmoreCommandProperty = BindableProperty.Create(nameof(OnLoadmoreCommand), typeof(ICommand), typeof(VerticalListView), null);
        public static readonly BindableProperty OnRefreshCommandProperty = BindableProperty.Create(nameof(OnRefreshCommand), typeof(ICommand), typeof(VerticalListView), null);

        private static INavigationService _navigationService;
        private static IDialogService _dialogService;
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public VerticalListView()
        {
            InitializeComponent();

            _navigationService = DependencyService.Resolve<INavigationService>();
            _dialogService = DependencyService.Resolve<IDialogService>();
        }

        public static ListViewModel ViewModel { get; set; } = new ListViewModel();

        public LearnerObservableCollection<ItemCard> Source
        {
            get { return (LearnerObservableCollection<ItemCard>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        public bool IsRemoveWhenUnBookmark
        {
            get { return (bool)GetValue(IsRemoveWhenUnBookmarkProperty); }
            set { SetValue(IsRemoveWhenUnBookmarkProperty, value); }
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

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (VerticalListView)bindable;

            var items = newValue as LearnerObservableCollection<ItemCard>;

            if (!items.Any())
            {
                view.ListItem.ItemsSource = null;
                view.EmptyViewMessage.Text = TextsResource.NOTHING_HERE_YET;
            }
            else
            {
                view.ListItem.ItemsSource = items;
            }

            /*KEEP FOR HANDLE LOAD MORE*/
            /*view.LoadingFooter.IsVisible = items.Count < view.TotalCount;*/
        }

        private static void OnTotalCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (VerticalListView)bindable;

            int totalCount = (int)newValue;

            if (totalCount == 0)
            {
                view.ListItem.IsVisible = false;
                view.EmptyViewMessage.IsVisible = true;
            }
            else
            {
                view.EmptyViewMessage.IsVisible = false;
                view.ListItem.IsVisible = true;
            }
        }

        private async void OnLoadmoreItem(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.Run(() =>
            {
                var view = sender as CollectionView;

                if (view.ItemsSource == null || OnLoadmoreCommand == null)
                {
                    return;
                }

                int currentCount = ((LearnerObservableCollection<ItemCard>)view.ItemsSource).Count;

                if (currentCount < TotalCount)
                {
                    OnLoadmoreCommand.Execute(null);
                }
            });
        }

        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var view = sender as CollectionView;

            if (!(view.SelectedItem is ItemCard itemSelected))
            {
                return;
            }

            using (_dialogService.DisplayLoadingIndicator())
            {
                switch (itemSelected.CardType)
                {
                    case BookmarkType.Microlearning:
                    case BookmarkType.Course:
                        if (itemSelected != null &&
                            !itemSelected.IsExpired &&
                            !itemSelected.CourseStatus.Equals(nameof(StatusCourse.Unpublished)) &&
                            !itemSelected.CourseStatus.Equals(nameof(StatusLearning.Archived)))
                        {
                            _navigationService.NavigateToAsync<CourseDetailsViewModel>(CourseDetailsViewModel.GetNavigationParameters(itemSelected.Id, itemSelected.CardType));
                        }

                        view.SelectedItem = null;
                        break;
                    case BookmarkType.DigitalContent:
                        _navigationService.NavigateToAsync<MyDigitalContentDetailsViewModel>(MyDigitalContentDetailsViewModel.GetNavigationParameters(itemSelected.Id));
                        view.SelectedItem = null;
                        break;
                    case BookmarkType.Community:
                        var parameters = new NavigationParameters();
                        parameters.SetParameter("details-url", itemSelected.DetailUrl);
                        _navigationService.NavigateToAsync<CommunityDetailViewModel>(parameters);
                        view.SelectedItem = null;
                        break;
                    case BookmarkType.LearningPath:
                        var navParams = new NavigationParameters();
                        navParams.SetParameter("learning-path-id", itemSelected.Id);
                        navParams.SetParameter("is-my-own-learning-path", LearningPathType.RecommendationLearningPath);
                        _navigationService.NavigateToAsync<MyLearningPathsDetailsViewModel>(navParams);
                        view.SelectedItem = null;
                        break;
                    case BookmarkType.StandAloneForm:
                        var navigationParameter = new NavigationParameters();
                        navigationParameter.SetParameter("form-id", itemSelected.Id);
                        _navigationService.NavigateToAsync<StandAloneFormViewModel>(navigationParameter);
                        view.SelectedItem = null;
                        break;
                    default:
                        view.SelectedItem = null;
                        break;
                }
            }
        }

        private void OnRefreshListView(object sender, EventArgs e)
        {
            OnRefreshCommand?.Execute(null);

            (sender as RefreshView).IsRefreshing = false;
        }

        private void OnBookmarkTrigger(object sender, ItemCard e)
        {
            if (IsRemoveWhenUnBookmark)
            {
                Source.Remove(e);
                TotalCount--;

                if (TotalCount == 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        EmptyViewMessage.Text = TextsResource.NOTHING_HERE_YET;
                    });
                }
            }
        }
    }
}
