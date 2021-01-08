using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views.InputMethods;
using LearnerApp.Controls;
using LearnerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]

namespace LearnerApp.Droid.Renderers
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.EditorAction += Control_EditorAction;

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

        // Fires only for Soft Keyboard
        private void Control_EditorAction(object sender, Android.Widget.TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                if (sender is FormsEditTextBase formsEditTextBase)
                {
                    var editor = (CustomEditor)Element;
                    editor.Text = formsEditTextBase.Text + "\n";
                    e.Handled = true;
                }
            }
        }
    }
}
