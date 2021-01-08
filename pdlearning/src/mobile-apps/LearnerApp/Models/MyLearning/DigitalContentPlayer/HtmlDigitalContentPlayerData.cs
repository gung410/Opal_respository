using System;
using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.Controls;
using Plugin.HybridWebView.Shared.Delegates;
using Plugin.HybridWebView.Shared.Enumerations;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.Models.MyLearning.DigitalContentPlayer
{
    public class HtmlDigitalContentPlayerData : BaseDigitalContentPlayerData
    {
        private string _html;
        private string _accessToken;

        public HtmlDigitalContentPlayerData(string accessToken, string html)
        {
            _accessToken = accessToken;
            _html = html;
        }

        public override View GetContentView()
        {
            var content = new CustomHybridWebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ContentType = WebViewContentType.StringData,
                Source = GlobalSettings.WebViewCloudFrontAuthenticationHtml.Replace("{{RETURN_URL}}", GlobalSettings.WebViewLearningContentPlayer).Replace("{{TOKEN}}", _accessToken)
            };

            content.OnNavigationStarted += OnNavigationStarted;
            content.OnNavigationCompleted += OnInlineContentNavigationCompleted;

            return content;
        }

        protected override List<string> InnerGetBrokenLink()
        {
            return ExtractUrlFromContent.Extract(_html);
        }

        private async void OnInlineContentNavigationCompleted(object sender, string e)
        {
            if (e == GlobalSettings.WebViewLearningContentPlayer)
            {
                var card = sender as CustomHybridWebView;

                await card.InjectJavascriptAsync($"document.body.innerHTML='{_html}'");
            }
        }

        private async void OnNavigationStarted(object sender, DecisionHandlerDelegate e)
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

            if (e.Uri.EndsWith(GlobalSettings.WebViewLearningContentPlayer))
            {
                return;
            }

            e.Cancel = true;

            await DialogService.ConfirmAsync("Do you want to navigate on this page?", onConfirmed: async (confirmed) =>
            {
                if (confirmed)
                {
                    await Browser.OpenAsync(e.Uri, BrowserLaunchMode.External);
                }
            });
        }
    }
}
