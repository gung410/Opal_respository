using LearnerApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationPageRenderer))]

namespace LearnerApp.iOS.Renderers
{
    public class CustomNavigationPageRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Need to check this, otherwise the app will crash.
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                // We need this to avoid/not support Dark Mode.
                OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            }
        }
    }
}
