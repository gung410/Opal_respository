using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ListItemCard.HorizontalItem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HorizontalLearningPathItem : ContentView
    {
        public HorizontalLearningPathItem()
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
