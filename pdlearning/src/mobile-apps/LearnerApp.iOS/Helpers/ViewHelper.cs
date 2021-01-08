using UIKit;

namespace LearnerApp.iOS.Helpers
{
    public static class ViewHelper
    {
        public static UIViewController FindViewController(this UIView view)
        {
            UIResponder result = TraverseResponderChainToViewController(view);
            return result as UIViewController;
        }

        public static UIViewController FindPresentingViewController()
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController presentViewController = window.RootViewController;
            while (presentViewController.PresentedViewController != null)
            {
                presentViewController = presentViewController.PresentedViewController;
            }

            return presentViewController;
        }

        public static UIResponder TraverseResponderChainToViewController(UIResponder responder)
        {
            if (responder != null)
            {
                UIResponder nextResponder = responder.NextResponder;
                if (nextResponder is UIViewController)
                {
                    return nextResponder;
                }
                else if (responder is UIView)
                {
                    return TraverseResponderChainToViewController(nextResponder);
                }
            }

            return null;
        }
    }
}
