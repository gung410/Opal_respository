using System.Linq;
using LearnerApp.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(LearnerApp.iOS.Effects.UnselectedTabColorEffect), nameof(UnselectedTabColorEffect))]

namespace LearnerApp.iOS.Effects
{
    public class UnselectedTabColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var tabBar = Container.Subviews.OfType<UITabBar>().FirstOrDefault();
            if (tabBar == null)
            {
                return;
            }

            tabBar.UnselectedItemTintColor = UIColor.FromRGB(49, 53, 82);
        }

        protected override void OnDetached()
        {
        }
    }
}
