using System;
using System.Collections.ObjectModel;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Helper;
using LearnerApp.Models.Learner;
using LearnerApp.Models.MyLearning;
using LearnerApp.Services;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalLearningPath : ContentView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ObservableCollection<LearningPath>), typeof(HorizontalLearningPath), null, propertyChanged: OnSourceChanged);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(HorizontalLearningPath), null, propertyChanged: OnTitleChanged);
        public static readonly BindableProperty TotalCountProperty = BindableProperty.Create(nameof(TotalCount), typeof(int), typeof(HorizontalLearningPath), null, propertyChanged: OnTotalCountChanged);
        public static readonly BindableProperty MyLearningPathTypeProperty = BindableProperty.Create(nameof(MyLearningPathType), typeof(LearningPathType), typeof(HorizontalLearningPath), null);

        private static INavigationService _navigationService;
        private static bool _isBusy;
        private readonly ICommonServices _commonService;

        public HorizontalLearningPath()
        {
            InitializeComponent();

            _commonService = DependencyService.Resolve<ICommonServices>();
            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public ObservableCollection<LearningPath> Source
        {
            get { return (ObservableCollection<LearningPath>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        public LearningPathType MyLearningPathType
        {
            get { return (LearningPathType)GetValue(MyLearningPathTypeProperty); }
            set { SetValue(MyLearningPathTypeProperty, value); }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalLearningPath)bindable;

            var items = newValue as ObservableCollection<LearningPath>;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (!items.IsNullOrEmpty())
                {
                    bool isVisibleBookmark = PermissionHelper.GetPermissionForBookmark();
                    if (!isVisibleBookmark)
                    {
                        foreach (var item in items)
                        {
                            item.IsVisibleBookmark = false;
                        }
                    }

                    view.LearningPathCollectionView.ItemsSource = items.Skip(0).Take(GlobalSettings.MaxResultPerPage);
                    view.LearningPathCollectionView.IsVisible = true;
                }
                else
                {
                    view.LearningPathCollectionView.IsVisible = false;
                }
            });
        }

        private static void OnTotalCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalLearningPath)bindable;
            int total = (int)newValue;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.TitleLbl.Text = $"{SeparateStringByUppercase.Convert(view.Title)} ({total}) >";

                if (total > 0)
                {
                    view.EmpltListLbl.IsVisible = false;
                }
                else
                {
                    view.EmpltListLbl.IsVisible = true;
                }
            });
        }

        private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is string title))
            {
                return;
            }

            var view = (HorizontalLearningPath)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.TitleLbl.Text = $"{SeparateStringByUppercase.Convert(title)} (0) >";
            });
        }

        private async void Navigation_Tapped(object sender, System.EventArgs e)
        {
            if (Source.IsNullOrEmpty())
            {
                return;
            }

            var navParams = new NavigationParameters();

            navParams.SetParameter("learning-paths-page-title", Title);
            navParams.SetParameter("is-my-own-learning-path", MyLearningPathType);

            await _navigationService.NavigateToAsync<MyLearningPathsShowAllViewModel>(navParams);
        }

        private async void OnItemSelected_Tapped(object sender, EventArgs e)
        {
            LearningPath item = (e as TappedEventArgs).Parameter as LearningPath;

            var navParams = new NavigationParameters();
            navParams.SetParameter("learning-path-id", item.Id);
            navParams.SetParameter("is-my-own-learning-path", MyLearningPathType);

            await _navigationService.NavigateToAsync<MyLearningPathsDetailsViewModel>(navParams);
        }

        private async void OnItemBookmark_Tapped(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            LearningPath learningPathsBookmark = (e as TappedEventArgs).Parameter as LearningPath;

            learningPathsBookmark.BookmarkInfo = await _commonService.Bookmark(learningPathsBookmark.Id, MyLearningPathType == LearningPathType.RecommendationLearningPath ? BookmarkType.LearningPathLMM : BookmarkType.LearningPath, learningPathsBookmark.BookmarkInfo == null ? true : false);

            _isBusy = false;
        }
    }
}
