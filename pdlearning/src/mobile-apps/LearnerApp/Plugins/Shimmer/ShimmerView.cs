#pragma warning disable

using System.ComponentModel;
using Xamarin.Forms;

namespace LearnerApp.Plugins.Shimmer
{
  /// <summary>This is the view used to achieve shimmer custom view.</summary>
  public class ShimmerView : View
  {
    /// <summary>
    /// Identifies the <see cref="P:LearnerApp.Plugins.Shimmer.ShimmerView.CornerRadius" /> bindable property.
    /// </summary>
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof (CornerRadius), typeof (double), typeof (ShimmerView), (object) 0.0, BindingMode.Default);

    /// <summary>
    /// Gets or sets the corner radius of <see cref="T:LearnerApp.Plugins.Shimmer.ShimmerView" />.
    /// </summary>
    public double CornerRadius
    {
      get => (double) this.GetValue(ShimmerView.CornerRadiusProperty);
      set => this.SetValue(ShimmerView.CornerRadiusProperty, (object) value);
    }

    /// <summary>Gets a background color of shimmer view.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Color BackgroundColor
    {
      get => (Color) this.GetValue(VisualElement.BackgroundColorProperty);
      internal set => this.SetValue(VisualElement.BackgroundColorProperty, (object) value);
    }

    /// <summary>
    /// Method that measures the view when a layout measurement happens.
    /// </summary>
    /// <param name="widthConstraint">Represents available width for shimmer view.</param>
    /// <param name="heightConstraint">Represents available height for shimmer view.</param>
    /// <returns>Retuns shimmer view measured size.</returns>
    protected override SizeRequest OnMeasure(
      double widthConstraint,
      double heightConstraint)
    {
      return new SizeRequest(new Size(40.0, 40.0));
    }
  }
}
