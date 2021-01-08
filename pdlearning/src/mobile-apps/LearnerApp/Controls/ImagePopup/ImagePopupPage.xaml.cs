using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.ImagePopup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePopupPage
    {
        public ImagePopupPage(ImagePopupViewModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var screenHeight = App.Current.MainPage.Height;

            var newHeight = screenHeight * 0.7;
            var minimumHeight = 400d;

            if (newHeight <= minimumHeight)
            {
                newHeight = screenHeight > minimumHeight ? minimumHeight : screenHeight;
            }

            RootView.HeightRequest = newHeight;
        }
    }
}
