using System;
using System.Threading.Tasks;
using LearnerApp.Controls;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsView
    {
        private readonly NotificationsViewModel _viewModel;

        public NotificationsView()
        {
            InitializeComponent();

            _viewModel = BindingContext as NotificationsViewModel;
        }

        protected override bool OnBackButtonPressed() => true;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<NotificationsView>(this, "reload-new-notification");
            MessagingCenter.Send(this, "reload-new-notification");
        }

        private async void NotificationWebview_OnNavigationCompleted(object sender, string e)
        {
            var view = sender as CustomHybridWebView;

            if (e.Equals(_viewModel.SourceUrl))
            {
                var injectJavascriptAsync = $"submit('{_viewModel.IdmResponse.DirectLoginUrl}','{_viewModel.IdmResponse.UserId}','{_viewModel.IdmResponse.Token}', '{_viewModel.ReturnUrl}');";
                await view.InjectJavascriptAsync("setTimeout(function() {" + injectJavascriptAsync + "}, 3000);");
            }
            else
            {
                view.AddLocalCallback("On_Item_Selected_Handler", async (data) => await OnItemSelectedHandler(data));
                await view.InjectJavascriptAsync("window.Notification.onNotificationClick = On_Item_Selected_Handler;");
            }
        }

        private async Task OnItemSelectedHandler(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            await _viewModel.GoToItemDetails(data);
        }

        private void NotificationWebview_OnNavigationStarted(object sender, Plugin.HybridWebView.Shared.Delegates.DecisionHandlerDelegate e)
        {
            if (e == null || string.IsNullOrEmpty(e.Uri))
            {
                return;
            }

            bool isUrl = Uri.TryCreate(e.Uri, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isUrl)
            {
                return;
            }
        }
    }
}
