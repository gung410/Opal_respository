using LearnerApp.Controls;
using LearnerApp.iOS.Renderers;
using Plugin.HybridWebView.iOS;
using Plugin.HybridWebView.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebViewVideoPlayer), typeof(WebViewVideoPlayerRenderer))]

namespace LearnerApp.iOS.Renderers
{
    public class WebViewVideoPlayerRenderer : HybridWebViewRenderer
    {
        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return new SizeRequest(Size.Zero, Size.Zero);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebViewControl> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.ScrollView.Bounces = false;
            }
        }
    }
}
