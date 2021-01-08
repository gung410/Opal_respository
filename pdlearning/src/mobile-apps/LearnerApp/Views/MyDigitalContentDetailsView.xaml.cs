using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDigitalContentDetailsView
    {
        public MyDigitalContentDetailsView()
        {
            InitializeComponent();
        }

        private async void Information_Clicked(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                InformationTabLine.IsVisible = true;
                ReviewsTabLine.IsVisible = false;
                await ScrollStack.ScrollToAsync(InformationGroup, ScrollToPosition.Start, true);
            });
        }

        private async void Reviews_Clicked(object sender, EventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                ReviewsTabLine.IsVisible = true;
                InformationTabLine.IsVisible = false;
                await ScrollStack.ScrollToAsync(ReviewsGroup, ScrollToPosition.Start, true);
            });
        }
    }
}
