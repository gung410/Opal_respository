using System;
using LearnerApp.Controls;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class WebViewVideoDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private static readonly string _redirectDigitalContentPlayerUrl = $"{GlobalSettings.WebViewUrlDigitalContentPlayer}?redirect=true";

        private readonly VideoData _videoData;
        private readonly string _accessToken;
        private WebViewVideoPlayer _videoViewer;

        public WebViewVideoDigitalContentPlayerData(string accessToken, VideoData videoData)
        {
            _videoData = videoData;
            _accessToken = accessToken;
        }

        public event Action<bool> OnDigitalContentPlayerCollapseOrExpand;

        public override View GetContentView()
        {
             _videoViewer = new WebViewVideoPlayer(_accessToken, _videoData)
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Source = _redirectDigitalContentPlayerUrl
            };

             return _videoViewer;
        }

        public override void LoadStyleForDigitalContentPlayer(Controls.DigitalContentPlayer.DigitalContentPlayer digitalContentPlayer)
        {
            base.LoadStyleForDigitalContentPlayer(digitalContentPlayer);

            digitalContentPlayer.Player.Padding = new Thickness(0);

            _videoViewer.OnPlayerCollapse += () =>
            {
                digitalContentPlayer.MetadataView.IsVisible = true;
                OnDigitalContentPlayerCollapseOrExpand?.Invoke(false);
            };

            _videoViewer.OnPlayerExpandFullScreen += () =>
            {
                digitalContentPlayer.MetadataView.IsVisible = false;
                OnDigitalContentPlayerCollapseOrExpand?.Invoke(true);
            };
        }
    }
}
