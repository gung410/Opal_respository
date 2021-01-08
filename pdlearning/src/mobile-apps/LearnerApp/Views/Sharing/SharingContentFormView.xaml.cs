using LearnerApp.Services.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Sharing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SharingContentFormView
    {
        public SharingContentFormView()
        {
            InitializeComponent();
        }

        private void OnContentScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                this.SearchBar.Unfocus();
            }
        }
    }
}
