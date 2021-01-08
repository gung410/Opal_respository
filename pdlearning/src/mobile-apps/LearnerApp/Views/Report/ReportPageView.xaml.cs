using LearnerApp.ViewModels.Report;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Report
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPageView
    {
        public ReportPageView()
        {
            InitializeComponent();
        }

        private ReportPageViewModel ViewModel => (ReportPageViewModel)BindingContext;

        protected override bool OnBackButtonPressed() => true;

        private async void SocialWebview_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var injectJavascriptAsync = $"submit('{ViewModel.IdmResponse.DirectLoginUrl}','{ViewModel.IdmResponse.UserId}','{ViewModel.IdmResponse.Token}', '{ViewModel.ReturnUrl}');";
            await WebView.EvaluateJavaScriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 3000);");
        }
    }
}
