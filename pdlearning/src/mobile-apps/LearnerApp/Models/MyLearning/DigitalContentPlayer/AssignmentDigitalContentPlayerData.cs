using System;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Controls;
using LearnerApp.Models.Course;
using Plugin.HybridWebView.Shared.Enumerations;
using Plugin.Toast;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class AssignmentDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private readonly string _accessToken;
        private readonly AssignmentDetail _assignmentData;

        private CustomHybridWebView _assignmentPlayer;
        private Controls.DigitalContentPlayer.DigitalContentPlayer _digitalContentPlayer;

        public AssignmentDigitalContentPlayerData(string accessToken, AssignmentDetail assignmentData)
        {
            _accessToken = accessToken;
            _assignmentData = assignmentData;
        }

        public event Action OnAssignmentBack;

        public event Action OnAssignmentSubmitted;

        public override View GetContentView()
        {
            _assignmentPlayer = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.Internet,
                Source = GlobalSettings.WebViewUrlAssignmentPlayer
            };

            return _assignmentPlayer;
        }

        public override void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            _digitalContentPlayer = digitalContentPlayer;
            _assignmentPlayer.OnNavigationStarted += (obj, e) => ShouldHandleUrlNavigation(e, GlobalSettings.WebViewUrlAssignmentPlayer);
            _assignmentPlayer.OnNavigationCompleted += OnAssignmentNavigationCompleted;
        }

        private async void OnAssignmentNavigationCompleted(object sender, string e)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await InitAssignmentPlayer();
                }
                else
                {
                    var observer = new Observer();
                    observer.CallAfter(
                        async () =>
                        {
                            using (DialogService.DisplayLoadingIndicator())
                            {
                                await InitAssignmentPlayer();
                            }
                        },
                        2000);
                }
            });
        }

        private async Task InitAssignmentPlayer()
        {
            if (_digitalContentPlayer == null)
            {
                return;
            }

            var card = _assignmentPlayer;
            await card.InjectJavascriptAsync($"AppGlobal.assignmentPlayerIntegrations.setAuthToken('{_accessToken}')");
            await card.InjectJavascriptAsync($"AppGlobal.assignmentPlayerIntegrations.setAssignmentId('{_assignmentData.Id}')");
            await card.InjectJavascriptAsync($"AppGlobal.assignmentPlayerIntegrations.setParticipantAssignmentTrackId('{_assignmentData.ParticipantAssignmentTrackId}')");
            await card.InjectJavascriptAsync($"AppGlobal.assignmentPlayerIntegrations.setIsMobile('true')");

            card.AddLocalCallback("On_Assignment_Back", (data) =>
            {
                OnAssignmentBack?.Invoke();
            });
            await card.InjectJavascriptAsync("AppGlobal.assignmentPlayerIntegrations.onAssignmentBack = On_Assignment_Back;");

            card.AddLocalCallback("On_Assignment_Submitted", (data) =>
            {
                OnAssignmentSubmitted?.Invoke();
                CrossToastPopUp.Current.ShowToastSuccess("Assignment submitted successfully");
            });
            await card.InjectJavascriptAsync("AppGlobal.assignmentPlayerIntegrations.onAssignmentSubmitted = On_Assignment_Submitted;");

            card.AddLocalCallback("On_Assignment_Saved", (data) =>
            {
                CrossToastPopUp.Current.ShowToastSuccess("Assignment saved successfully");
            });
            await card.InjectJavascriptAsync("AppGlobal.assignmentPlayerIntegrations.onAssignmentSaved = On_Assignment_Saved;");
        }
    }
}
