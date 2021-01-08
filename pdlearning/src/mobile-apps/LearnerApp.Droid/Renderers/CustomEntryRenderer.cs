using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using LearnerApp.Controls;
using LearnerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace LearnerApp.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackground(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                this.Control.SetPadding(13, 14, 0, 14);

                // Remove selection text
                this.Control.CustomSelectionActionModeCallback = new CustomSelectionActionModeCallback();
                this.Control.LongClickable = false;
            }
        }
    }
}
