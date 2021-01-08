using System;
using LearnerApp.Common.Helper;
using LearnerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.VerticalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerticalDigitalContentItem : ContentView
    {
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public VerticalDigitalContentItem()
        {
            InitializeComponent();
        }

        public event EventHandler<ItemCard> BookmarkRemoveItemTrigger;

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
