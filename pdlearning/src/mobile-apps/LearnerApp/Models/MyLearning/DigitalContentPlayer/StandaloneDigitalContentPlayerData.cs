using System;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Controls;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class StandaloneDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        public Action<string> OnQuizFinished;

        private readonly string _accessToken;
        private readonly string _standaloneFormId;

        public StandaloneDigitalContentPlayerData(string accessToken, string standaloneFormId)
        {
            _accessToken = accessToken;
            _standaloneFormId = standaloneFormId;
        }

        public override View GetContentView()
        {
            var standAloneFormContent = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.StringData,
                Source = GlobalSettings.WebViewCloudFrontAuthenticationHtml.Replace("{{RETURN_URL}}", GlobalSettings.WebViewUrlStandAloneFormPlayer).Replace("{{TOKEN}}", _accessToken)
            };

            standAloneFormContent.OnNavigationStarted += (sender, args) => ShouldHandleUrlNavigation(args, GlobalSettings.WebViewUrlStandAloneFormPlayer);
            standAloneFormContent.OnNavigationCompleted += OnStandAloneFormNavigationCompleted;

            return standAloneFormContent;
        }

        private async void OnStandAloneFormNavigationCompleted(object sender, string e)
        {
            if (e.Contains(GlobalSettings.WebViewUrlStandAloneFormPlayer))
            {
                var card = sender as CustomHybridWebView;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    await CommonInit(card);
                }
                else
                {
                    var observer = new Observer();
                    observer.CallAfter(
                        async () =>
                        {
                            using (DialogService.DisplayLoadingIndicator())
                            {
                                await CommonInit(card);
                            }
                        },
                        2000);
                }
            }
        }

        private async Task CommonInit(CustomHybridWebView card)
        {
            await InitStandAlonePlayer(card);
            card.AddLocalCallback("CCPM_Mobile_On_Quiz_Finished_Handler", async (data) =>
            {
                OnQuizFinishHandler(data);
                await InitStandAlonePlayer(card);
            });

            await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizFinishedForMobile = CCPM_Mobile_On_Quiz_Finished_Handler;");
            card.AddLocalCallback("CCPM_Mobile_On_Quiz_Submitted_Handler", (data) => { });
            await card.InjectJavascriptAsync("AppGlobal.quizPlayerIntegrations.onQuizSubmitted = CCPM_Mobile_On_Quiz_Submitted_Handler;");
        }

        private async Task InitStandAlonePlayer(CustomHybridWebView card)
        {
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setAuthToken('{_accessToken}')");
            await card.InjectJavascriptAsync($"AppGlobal.quizPlayerIntegrations.setFormOriginalObjectId('{_standaloneFormId}')");
        }

        private void OnQuizFinishHandler(string data)
        {
            OnQuizFinished?.Invoke(data);
        }
    }
}
