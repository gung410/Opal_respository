using System;
using System.Windows.Input;
using LearnerApp.Common.Helper;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models.Newsfeed;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticalNewsfeedList : ContentView
    {
        public static readonly BindableProperty SourceItemsProperty = BindableProperty.Create(nameof(SourceItems), typeof(LearnerObservableCollection<Feed>), typeof(VerticalNewsfeedList), null, propertyChanged: OnSourceItemsChanged);
        public static readonly BindableProperty TotalCountProperty = BindableProperty.Create(nameof(TotalCount), typeof(int), typeof(VerticalNewsfeedList));
        public static readonly BindableProperty LoadmoreCommandProperty = BindableProperty.Create(nameof(LoadmoreCommand), typeof(ICommand), typeof(VerticalNewsfeedList));
        public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(VerticalNewsfeedList));

        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public VerticalNewsfeedList()
        {
            InitializeComponent();
        }

        public ICommand LoadmoreCommand
        {
            get { return (ICommand)GetValue(LoadmoreCommandProperty); }
            set { SetValue(LoadmoreCommandProperty, value); }
        }

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public LearnerObservableCollection<Feed> SourceItems
        {
            get { return (LearnerObservableCollection<Feed>)GetValue(SourceItemsProperty); }
            set { SetValue(SourceItemsProperty, value); }
        }

        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        private static void OnSourceItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var items = newValue as LearnerObservableCollection<Feed>;
            if (bindable is VerticalNewsfeedList view)
            {
                view.CollectionSource.ItemsSource = items;
            }
        }

        private async void SourceCollection_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            if (((sender as CollectionView).ItemsSource as LearnerObservableCollection<Feed>).Count < TotalCount)
            {
                await _loadMoreStressActionHandler.Run(() =>
                {
                    LoadmoreCommand?.Execute(null);
                });
            }
            else
            {
                CollectionSource.RemainingItemsThreshold = -1;
            }
        }

        private void RefreshView_Refreshing(object sender, EventArgs e)
        {
            RefreshCommand?.Execute(null);
            CollectionSource.RemainingItemsThreshold = 0;
            ((RefreshView)sender).IsRefreshing = false;
        }
    }
}
