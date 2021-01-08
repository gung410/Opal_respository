using System;
using System.Collections.ObjectModel;
using LearnerApp.Common;
using LearnerApp.Common.Helper;
using LearnerApp.Models;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalListView : ContentView
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ObservableCollection<ItemCard>), typeof(HorizontalListView), null, propertyChanged: OnSourceChanged);
        public static readonly BindableProperty EmplyListMessageProperty = BindableProperty.Create(nameof(EmptyListMessage), typeof(string), typeof(HorizontalListView), null, propertyChanged: OnEmptyListMessageChanged);
        public static readonly BindableProperty NavigationTitleProperty = BindableProperty.Create(nameof(NavigationTitle), typeof(string), typeof(HorizontalListView), null, propertyChanged: OnNavigationTitleChanged);
        public static readonly BindableProperty TotalCountProperty = BindableProperty.Create(nameof(TotalCount), typeof(int), typeof(HorizontalListView), -1, propertyChanged: OnTotalCountChanged);
        public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(HorizontalListView), null, propertyChanged: OnIsLoadingChanged);
        public static readonly BindableProperty NavigationKeyProperty = BindableProperty.Create(nameof(NavigationKey), typeof(string), typeof(HorizontalListView), null);

        private static INavigationService _navigationService;
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public HorizontalListView()
        {
            InitializeComponent();

            _navigationService = DependencyService.Resolve<INavigationService>();
        }

        public static ListViewModel ViewModel { get; set; } = new ListViewModel();

        public ObservableCollection<ItemCard> Source
        {
            get { return (ObservableCollection<ItemCard>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        public string NavigationTitle
        {
            get { return (string)GetValue(NavigationTitleProperty); }
            set { SetValue(NavigationTitleProperty, value); }
        }

        public string EmptyListMessage
        {
            get { return (string)GetValue(EmplyListMessageProperty); }
            set { SetValue(EmplyListMessageProperty, value); }
        }

        public string NavigationKey
        {
            get { return (string)GetValue(NavigationKeyProperty); }
            set { SetValue(NavigationKeyProperty, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalListView)bindable;

            var items = newValue as ObservableCollection<ItemCard>;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.ListItem.RealItemsSource = items;
            });
        }

        private static void OnEmptyListMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalListView)bindable;
            string message = newValue as string;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.EmpltListLbl.Text = message;
            });
        }

        private static void OnTotalCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            int count = (int)newValue;
            var view = (HorizontalListView)bindable;

            view.TitleLbl.Text = $"{SeparateStringByUppercase.Convert(view.NavigationTitle)} ({count}) >";
            UpdateVisibilityOfEmptyLabel(view);
        }

        private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalListView)bindable;
            UpdateVisibilityOfEmptyLabel(view);
        }

        private static void UpdateVisibilityOfEmptyLabel(HorizontalListView list)
        {
            if (list.TotalCount == 0 && !list.IsLoading)
            {
                list.EmpltListLbl.IsVisible = true;
                list.ListItem.IsVisible = false;
            }
            else
            {
                list.EmpltListLbl.IsVisible = false;
                list.ListItem.IsVisible = true;
            }
        }

        private static void OnNavigationTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (HorizontalListView)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.TitleLbl.Text = $"{newValue as string} (0) >";
            });
        }

        private async void Navigation_Tapped(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                if (Source.Count > 0)
                {
                    var parameters = new NavigationParameters();
                    parameters.SetParameter("SourceScreen", NavigationKey);
                    parameters.SetParameter("ForceNavigation", true);

                    switch (NavigationKey)
                    {
                        case "MyLearning":
                            await _navigationService.NavigateToMainTabItem(NavigationRoutes.MyLearning, parameters);
                            break;
                        case "Bookmarks":
                            await _navigationService.NavigateToMainTabItem(NavigationRoutes.MyLearning, parameters);
                            break;
                        case "NewlyAdded":
                            await _navigationService.NavigateToAsync<CoursesViewModel>(parameters);
                            break;
                        case "RecommendationsForYou":
                            await _navigationService.NavigateToAsync<CoursesViewModel>(parameters);
                            break;
                        case "RecommendationByYourOrganisation":
                            await _navigationService.NavigateToAsync<CoursesViewModel>(parameters);
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}
