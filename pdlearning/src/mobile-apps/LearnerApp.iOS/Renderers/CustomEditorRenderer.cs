using Foundation;
using LearnerApp.Controls;
using LearnerApp.iOS.Renderers;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]

namespace LearnerApp.iOS.Renderers
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public override bool CanPerform(Selector action, NSObject withSender)
        {
            NSOperationQueue.MainQueue.AddOperation(() =>
            {
                UIMenuController.SharedMenuController.SetMenuVisible(false, false);
            });

            return base.CanPerform(action, withSender);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Layer.BorderWidth = 0.0f;
                Control.Layer.BorderColor = UIColor.White.CGColor;
            }
        }
    }
}
