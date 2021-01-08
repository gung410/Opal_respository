using System;
using LearnerApp.Common;
using LearnerApp.Models.MyLearning;
using Newtonsoft.Json;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewVideoPlayer
    {
        public Action OnPlayerCollapse;

        public Action OnPlayerExpandFullScreen;

        private readonly VideoData _videoData;
        private readonly string _accessToken;

        private bool _firstNavigated = true;

        public WebViewVideoPlayer(string accessToken, VideoData videoData)
        {
            InitializeComponent();
            _videoData = videoData;
            _accessToken = accessToken;
            OnNavigationCompleted += OnInnerNavigationCompleted;
        }

        private void OnInnerNavigationCompleted(object sender, string e)
        {
            if (e != Source)
            {
                return;
            }

            if (_firstNavigated)
            {
                var arguments = string.Join(
                    ",",
                    new string[]
                    {
                        BuildJavascriptArgumentString(_accessToken),
                        BuildJavascriptArgumentString(_videoData.DigitalContentId),
                        BuildJavascriptArgumentString(_videoData.LectureId),
                        BuildJavascriptArgumentString(_videoData.ClassRunId),
                        BuildJavascriptArgumentString(_videoData.MyLectureId),
                        BuildJavascriptArgumentString("learn"),
                        "true",
                        "fullscreenCallback"
                    });

                var injectJs = $"AppGlobal.digitalContentPlayerIntergrations.init({arguments})";
                this.InjectJavascriptAsync("var fullscreenCallback = (data) => { innerInvoke(data) }");
                this.InjectJavascriptAsync(injectJs);
                _firstNavigated = false;
            }
            else
            {
                AddLocalCallback("innerInvoke", data =>
                {
                    var isFullScreen = bool.Parse(data);
                    if (isFullScreen)
                    {
                        OpenPlayerViewAsFullScreen();
                    }
                    else
                    {
                        CollapsePlayer();
                    }
                });
            }
        }

        private string BuildJavascriptArgumentString(string argument)
        {
            return argument.IsNullOrEmpty() ? "\"\"" : $"\"{argument}\"";
        }

        private void CollapsePlayer()
        {
            OnPlayerCollapse?.Invoke();
        }

        private void OpenPlayerViewAsFullScreen()
        {
            OnPlayerExpandFullScreen?.Invoke();
        }
    }
}
