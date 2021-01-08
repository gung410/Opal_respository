using LearnerApp.ViewModels;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabRootContentPage : ContentPage
    {
        private bool _firstAppearing = true;

        public TabRootContentPage()
        {
            InitializeComponent();
            this.CustomNavigationView.BindingContext = MainTopbarViewModel.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_firstAppearing == false)
            {
                if (BindingContext is BasePageViewModel basePageViewModel)
                {
                    basePageViewModel.OnComeBack();
                }
            }

            _firstAppearing = false;
        }
    }
}
