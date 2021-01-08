using System.Diagnostics;
using Android.Webkit;

namespace LearnerApp.Droid.Renderers
{
    public class CustomWebViewClient : WebViewClient
    {
        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            Debug.WriteLine("Loading website...");
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            Debug.WriteLine("Load finished.");
        }

        /// <inheritdoc />
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            Debug.WriteLine("Url Loading.");
            return base.ShouldOverrideUrlLoading(view, request);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);

            Debug.WriteLine(error);
        }
    }
}
