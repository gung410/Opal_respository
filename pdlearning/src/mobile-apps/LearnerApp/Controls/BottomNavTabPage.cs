using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace LearnerApp.Controls
{
    public class BottomNavTabPage : Xamarin.Forms.TabbedPage
    {
        public static readonly BindableProperty ShiftingEnabledProperty = BindableProperty.Create(
            nameof(IsPageChangeEnabled),
            typeof(bool),
            typeof(BottomNavTabPage),
            false);

        public BottomNavTabPage()
        {
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        public bool IsPageChangeEnabled
        {
            get { return (bool)GetValue(ShiftingEnabledProperty); }
            set { SetValue(ShiftingEnabledProperty, value); }
        }

        protected override void OnAppearing()
        {
            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(IsPageChangeEnabled);
            base.OnAppearing();
        }
    }
}
