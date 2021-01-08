using Android.OS;
using Android.Views;
using Android.Webkit;
using LearnerApp.Controls;
using LearnerApp.Droid.Renderers;
using Plugin.HybridWebView.Droid;
using Plugin.HybridWebView.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomHybridWebView), typeof(CustomHybridWebViewRenderer))]

namespace LearnerApp.Droid.Renderers
{
    public class CustomHybridWebViewRenderer : HybridWebViewRenderer
    {
        public override bool DispatchTouchEvent(MotionEvent e)
        {
            Parent.RequestDisallowInterceptTouchEvent(true);
            return base.DispatchTouchEvent(e);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebViewControl> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Set this for CloudFront Cookie.
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    CookieManager.Instance.SetAcceptThirdPartyCookies(Control, true);
                }
                else
                {
                    CookieManager.Instance.SetAcceptCookie(true);
                }

                Control.Settings.SetAppCacheEnabled(false);
                Control.Settings.CacheMode = CacheModes.NoCache;
                Control.SetWebChromeClient(new CustomHybridWebViewChromeClient(this));
            }
        }
    }
}
