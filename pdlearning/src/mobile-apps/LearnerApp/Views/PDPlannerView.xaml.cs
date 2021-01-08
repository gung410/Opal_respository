using System;
using LearnerApp.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class PDPlannerView
    {
        public PDPlannerView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() => true;

        private async void WebView_OnOnContentLoaded(object sender, EventArgs e)
        {
            var accessToken = await ((PDPlannerViewModel)this.BindingContext).GetAccessToken();

            var dataMessage = new { action = "SET_ACCESS_TOKEN", payload = new { accessToken } };

            string message = JsonConvert.SerializeObject(dataMessage);

            string injectJavascript = "(function () {" + $"window.onExternalMessage(" + message + ");" + "})()";

            await WebView.InjectJavascriptAsync(injectJavascript);
        }
    }
}
