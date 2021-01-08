using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using LearnerApp.Controls;
using LearnerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]

namespace LearnerApp.Droid.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, Android.Webkit.WebView>
    {
        private readonly Context _localContext;

        public CustomWebViewRenderer(Context context) : base(context)
        {
            _localContext = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            var webView = Control;

            if (Control == null)
            {
                webView = new Android.Webkit.WebView(_localContext);
                SetNativeControl(webView);
            }

            // Set this for CloudFront Cookie.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                CookieManager.Instance.SetAcceptThirdPartyCookies(webView, true);
            }
            else
            {
                CookieManager.Instance.SetAcceptCookie(true);
            }

            if (e.NewElement != null)
            {
                webView.Settings.JavaScriptEnabled = true;

                webView.Settings.BuiltInZoomControls = true;

                webView.Settings.SetSupportZoom(true);

                webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
                webView.ScrollbarFadingEnabled = false;

                webView.SetWebViewClient(new CustomWebViewClient());

                UrlWebViewSource source = Element.Source as UrlWebViewSource;

                if (!string.IsNullOrEmpty(source?.Url))
                {
                    webView.LoadUrl(source.Url);
                }
            }
        }
    }
}
