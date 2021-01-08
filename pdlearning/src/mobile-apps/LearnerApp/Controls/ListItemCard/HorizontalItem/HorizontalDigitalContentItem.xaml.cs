using System;
using LearnerApp.Common.Helper;
using LearnerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.HorizontalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalDigitalContentItem : ContentView
    {
        private readonly StressActionHandler _loadMoreStressActionHandler = new StressActionHandler(TimeSpan.FromSeconds(2));

        public HorizontalDigitalContentItem()
        {
            InitializeComponent();
        }

        private async void Item_Selected(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var data = (e as TappedEventArgs)?.Parameter as ItemCard;
                await HorizontalListView.ViewModel.ItemSelected(data);
            });
        }

        private async void Bookmark_Tapped(object sender, EventArgs e)
        {
            await _loadMoreStressActionHandler.RunAsync(async () =>
            {
                var data = (e as TappedEventArgs)?.Parameter as ItemCard;
                await HorizontalListView.ViewModel.ItemBookmarked(data);
            });
        }
    }
}
