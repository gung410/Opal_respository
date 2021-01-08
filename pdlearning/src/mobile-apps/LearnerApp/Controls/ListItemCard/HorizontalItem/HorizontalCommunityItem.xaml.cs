using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.HorizontalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalCommunityItem : ContentView
    {
        public HorizontalCommunityItem()
        {
            InitializeComponent();
        }

        private void Item_Selected(object sender, EventArgs e)
        {
        }

        private void Bookmark_Tapped(object sender, EventArgs e)
        {
        }
    }
}
