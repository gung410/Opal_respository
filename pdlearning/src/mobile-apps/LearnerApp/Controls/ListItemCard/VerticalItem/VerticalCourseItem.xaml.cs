using System;
using LearnerApp.Common.Helper;
using LearnerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.VerticalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticalCourseItem : ContentView
    {
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public VerticalCourseItem()
        {
            InitializeComponent();
        }

        public event EventHandler<ItemCard> BookmarkRemoveItemTrigger;

        private async void DeclineOffer_Clicked(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var item = (sender as Button).CommandParameter as ItemCard;
                await VerticalListView.ViewModel.DeclineOffer(item);

                (((sender as Button).Parent as StackLayout).Parent as StackLayout).IsVisible = false;
            });
        }

        private async void AccessOffer_Clicked(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var item = (sender as Button).CommandParameter as ItemCard;
                await VerticalListView.ViewModel.AccessOffer(item);

                (((sender as Button).Parent as StackLayout).Parent as StackLayout).IsVisible = false;
            });
        }

        private async void ShowRejectComment_Clicked(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var item = (sender as Button).CommandParameter as ItemCard;
                await VerticalListView.ViewModel.ShowRejectComment(item);
            });
        }

        private async void OnBookmarkItem(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var item = (e as TappedEventArgs).Parameter as ItemCard;
                await VerticalListView.ViewModel.ItemBookmarked(item);

                BookmarkRemoveItemTrigger?.Invoke(this, item);
            });
        }
    }
}
