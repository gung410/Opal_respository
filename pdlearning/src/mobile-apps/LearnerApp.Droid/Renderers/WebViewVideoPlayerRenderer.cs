using LearnerApp.Controls;
using LearnerApp.Droid.Renderers;
using Plugin.HybridWebView.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(WebViewVideoPlayer), typeof(WebViewVideoPlayerRenderer))]

namespace LearnerApp.Droid.Renderers
{
    public class WebViewVideoPlayerRenderer : HybridWebViewRenderer
    {
        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return new SizeRequest(Size.Zero, Size.Zero);
        }
    }
}
