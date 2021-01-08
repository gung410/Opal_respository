using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EportfolioView
    {
        private EportfolioViewModel _viewModel;

        public EportfolioView()
        {
            InitializeComponent();

            _viewModel = (EportfolioViewModel)BindingContext;
        }

        protected override bool OnBackButtonPressed() => true;

        private async void EportfolioWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
            await EportfolioWebView.EvaluateJavaScriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 3000);");
        }
    }
}
