using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Controls;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class DocumentViewerDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private readonly string _source;

        public DocumentViewerDigitalContentPlayerData(string source)
        {
            _source = source;
        }

        public override View GetContentView()
        {
            var documentViewer = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.Internet,
                Source = _source
            };
            documentViewer.OnNavigationStarted += (sender, args) => ShouldHandleUrlNavigation(
                args,
                GlobalSettings.WebViewUrlDigitalContentPlayer,
                GlobalSettings.WebViewGoogleDocumentViewer);
            documentViewer.OnNavigationCompleted += OnDocumentViewerNavigationCompleted;

            return documentViewer;
        }

        protected override List<string> InnerGetBrokenLink()
        {
            return new List<string>()
            {
                _source
            };
        }

        private async void OnDocumentViewerNavigationCompleted(object sender, string e)
        {
            using (DialogService.DisplayLoadingIndicator())
            {
                var card = sender as CustomHybridWebView;

                if (card.CurrentUrl.EndsWith(GlobalSettings.WebViewUrlDigitalContentPlayer))
                {
                    return;
                }

                string googleToolbarSelector = "document.querySelector('[role=\"toolbar\"]')";
                var found = await card.InjectJavascriptAsync(googleToolbarSelector);

                if (found == "null" || found == "<null>")
                {
                    // The web-view difference behavior between android and iOS
                    // We must delay about 1 second in Android to reload document viewer if content null
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(1000);
                    }

                    var version = $"&v={new Random().Next(1, 99999)}";
                    card.Source = GlobalSettings.WebViewGoogleDocumentViewer + System.Web.HttpUtility.UrlEncode(_source) + "&embedded=true" + version;
                }
                else
                {
                    await card.InjectJavascriptAsync(googleToolbarSelector + ".remove()");
                }
            }
        }
    }
}
