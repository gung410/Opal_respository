#pragma  warning disable

using LearnerApp.iOS.Renderers.Plugin;
using LearnerApp.Plugins.Shimmer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ShimmerView), typeof(ShimmerViewRenderer))]
namespace LearnerApp.iOS.Renderers.Plugin
{
    internal class ShimmerViewRenderer : ViewRenderer
    {
    }
}
