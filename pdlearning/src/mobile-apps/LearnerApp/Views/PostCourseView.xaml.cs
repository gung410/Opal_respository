using System;
using System.Threading.Tasks;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels;
using LearnerApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostCourseView
    {
        private readonly PostCourseViewModel _viewModel;
        private readonly IIdentityService _identityService;

        public PostCourseView()
        {
            InitializeComponent();

            _viewModel = (PostCourseViewModel)BindingContext;

            DialogService = DependencyService.Resolve<IDialogService>();
            _identityService = DependencyService.Resolve<IIdentityService>();
        }

        protected IDialogService DialogService { get; }

        private async void WebviewCard_OnNavigationLoaded(object sender, EventArgs e)
        {
            string accessToken = (await _identityService.GetAccountPropertiesAsync()).AccessToken;

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    using (DialogService.DisplayLoadingIndicator())
                    {
                        await WebviewCard.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setAuthToken('{accessToken}')");
                        await WebviewCard.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setFormId('{_viewModel.FormId}')");
                        await WebviewCard.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setResourceId('{_viewModel.ResourceId}')");

                        WebviewCard.AddLocalCallback("CCPM_Mobile_On_Quiz_Finished_Handler", async (data) => await CCPM_Mobile_On_Scorm_Finished_Handler(data));
                        await WebviewCard.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile = CCPM_Mobile_On_Quiz_Finished_Handler;");

                        WebviewCard.AddLocalCallback("CCPM_Mobile_On_Quiz_Session_Expired_Handler", async (data) => await CCPM_Mobile_On_Quiz_Session_Expired_Handler(data));
                        await WebviewCard.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.onAuthTokenError = CCPM_Mobile_On_Quiz_Session_Expired_Handler;");
                    }
                }
                catch (Exception ex)
                {
                    await DialogService.ShowAlertAsync(ex.Message, cancelTextBtn: "Try again");
                }
            });
        }

        private Task CCPM_Mobile_On_Quiz_Session_Expired_Handler(string data)
        {
            return Task.CompletedTask;
        }

        private async Task CCPM_Mobile_On_Scorm_Finished_Handler(string data)
        {
            await _viewModel.CompleteCourse();
        }
    }
}
