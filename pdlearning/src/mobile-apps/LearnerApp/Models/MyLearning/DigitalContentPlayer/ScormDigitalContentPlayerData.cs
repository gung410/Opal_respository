using System;
using LearnerApp.Common;
using LearnerApp.Controls;
using Plugin.DeviceOrientation;
using Plugin.DeviceOrientation.Abstractions;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class ScormDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        public Action OnScormCompleted;

        private readonly ScormData _scormData;
        private readonly string _accessToken;

        private Controls.DigitalContentPlayer.DigitalContentPlayer _digitalContentPlayer;
        private CustomHybridWebView _scormPlayer;

        public ScormDigitalContentPlayerData(string accessToken, ScormData scormData)
        {
            _accessToken = accessToken;
            _scormData = scormData;
        }

        public override View GetContentView()
        {
            _scormPlayer = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.StringData,
                Source = GlobalSettings.WebViewCloudFrontAuthenticationHtml.Replace("{{RETURN_URL}}", GlobalSettings.WebViewUrlScormPlayer).Replace("{{TOKEN}}", _accessToken)
            };
            _scormPlayer.OnNavigationStarted += (obj, args) =>
                ShouldHandleUrlNavigation(
                    args,
                    GlobalSettings.WebViewUrlScormPlayer,
                    GlobalSettings.WebViewUrlScormPlayer,
                    $"{GlobalSettings.WebViewUrlScormPlayer}?redirect=true");

            _scormPlayer.OnNavigationCompleted += OnScormNavigationCompleted;

            var scroll = new ScrollView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = _scormPlayer
            };

            return scroll;
        }

        public override void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            base.LoadStyleForDigitalContentPlayer(digitalContentPlayer);
            _digitalContentPlayer = digitalContentPlayer;

            CrossDeviceOrientation.Current.UnlockOrientation();
            CrossDeviceOrientation.Current.OrientationChanged += (sender, e) =>
            {
                var position = _scormPlayer.HeightRequest + _scormPlayer.WidthRequest;
                _scormPlayer.HeightRequest = position - _scormPlayer.Height;
                _scormPlayer.WidthRequest = position - _scormPlayer.Width;
            };
        }

        public override void OnClearPlayer()
        {
            base.OnClearPlayer();
            CrossDeviceOrientation.Current.LockOrientation(DeviceOrientations.Portrait);
        }

        private async void OnScormNavigationCompleted(object sender, string e)
        {
            var card = sender as CustomHybridWebView;

            if (e != GlobalSettings.WebViewUrlScormPlayer)
            {
                return;
            }

            string contentIdProperty = _scormData.ScormType == ScormType.LearningContent ? "myLectureId" : "myDigitalContentId";

            card.AddLocalCallback("OnScormPlayerFinishHandler", (data) => OnScormPlayerFinishHandler());
            var injectScript = $"window.ScormPlayer.initData = {{ displayMode: 'learn', accessToken: '{_accessToken}', contentApiUrl: '{GlobalSettings.BackendServiceContent}/api', learnerApiUrl: '{GlobalSettings.BackendServiceLearner}/api', cloudFrontUrl: '{GlobalSettings.CloudFrontUrl}', digitalContentId: '{_scormData.DigitalContentId}', {contentIdProperty}: '{_scormData.MyLectureId}' }};window.ScormPlayer.accessToken='{_accessToken}';window.ScormPlayer.init();";
            var injectHandlerScript = "window.ScormPlayer.onScormPlayerFinishHandler = OnScormPlayerFinishHandler;";
            await card.InjectJavascriptAsync($"{injectScript}{injectHandlerScript}");
        }

        private void OnScormPlayerFinishHandler()
        {
            if (_scormData.ScormType == ScormType.DigitalContent)
            {
                OnScormCompleted?.Invoke();
            }
        }
    }
}
