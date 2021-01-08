using System.Windows.Input;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseLearnerContentPage
    {
        protected readonly INavigationService NavigationService;

        private bool _firstAppearing = true;

        public BaseLearnerContentPage()
        {
            InitializeComponent();
            NavigationService = DependencyService.Resolve<INavigationService>();
        }

        public ICommand BackCommand => new Command(async () =>
        {
            await NavigationService.GoBack();
        });

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
