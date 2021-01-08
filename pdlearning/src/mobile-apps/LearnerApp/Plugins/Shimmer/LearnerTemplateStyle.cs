#pragma warning disable
using Xamarin.Forms;

namespace LearnerApp.Plugins.Shimmer
{
  /// <summary>
  /// Serves as a base class for all template style class. This class provides options to customize radius, width and height of template.
  /// </summary>
  public class LearnerTemplateStyle : BindableObject
  {
    /// <summary>
    /// Identifies the radius bindable property.This property can be used to set radius for circle in the template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty CircleRadiusProperty = BindableProperty.Create(nameof (CircleRadius), typeof (double), typeof (LearnerTemplateStyle), (object) 0.0, BindingMode.Default);
    /// <summary>
    /// Identifies the BoxWidth bindable property.This property can be used to set width for box in the template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty SquareSizeProperty = BindableProperty.Create(nameof (SquareSize), typeof (double), typeof (LearnerTemplateStyle), (object) 0.0, BindingMode.Default);
    /// <summary>
    /// Identifies the width bindable property.This property can be used to set width of template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty WidthProperty = BindableProperty.Create(nameof (Width), typeof (double), typeof (LearnerTemplateStyle), (object) -1.0, BindingMode.Default);
    /// <summary>
    /// Identifies the height bindable property.This property can be used to set height of template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty HeightProperty = BindableProperty.Create(nameof (Height), typeof (double), typeof (LearnerTemplateStyle), (object) -1.0, BindingMode.Default);
    /// <summary>
    /// Identifies the item spacing bindable property.This property can be used to set spacing between template items.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty LineSpacingProperty = BindableProperty.Create(nameof (LineSpacing), typeof (double), typeof (LearnerTemplateStyle), (object) 0.0, BindingMode.Default);
    /// <summary>
    /// Identifies the item height bindable property.This property can be used to set height of template items.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty LineHeightProperty = BindableProperty.Create(nameof (LineHeight), typeof (double), typeof (LearnerTemplateStyle), (object) 0.0, BindingMode.Default);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:LearnerApp.Plugins.Shimmer.SfTemplateStyle" /> class.
    /// </summary>
    public LearnerTemplateStyle()
    {
    }

    /// <summary>Gets or sets the radius.</summary>
    public double CircleRadius
    {
      get => (double) this.GetValue(LearnerTemplateStyle.CircleRadiusProperty);
      set => this.SetValue(LearnerTemplateStyle.CircleRadiusProperty, (object) value);
    }

    /// <summary>Gets or sets the width.</summary>
    public double Width
    {
      get => (double) this.GetValue(LearnerTemplateStyle.WidthProperty);
      set => this.SetValue(LearnerTemplateStyle.WidthProperty, (object) value);
    }

    /// <summary>Gets or sets the height.</summary>
    public double Height
    {
      get => (double) this.GetValue(LearnerTemplateStyle.HeightProperty);
      set => this.SetValue(LearnerTemplateStyle.HeightProperty, (object) value);
    }

    /// <summary>Gets or sets the spacing between template items.</summary>
    public double LineSpacing
    {
      get => (double) this.GetValue(LearnerTemplateStyle.LineSpacingProperty);
      set => this.SetValue(LearnerTemplateStyle.LineSpacingProperty, (object) value);
    }

    /// <summary>Gets or sets the height of template items.</summary>
    public double LineHeight
    {
      get => (double) this.GetValue(LearnerTemplateStyle.LineHeightProperty);
      set => this.SetValue(LearnerTemplateStyle.LineHeightProperty, (object) value);
    }

    /// <summary>Gets or sets the width for box in the template.</summary>
    public double SquareSize
    {
      get => (double) this.GetValue(LearnerTemplateStyle.SquareSizeProperty);
      set => this.SetValue(LearnerTemplateStyle.SquareSizeProperty, (object) value);
    }
  }
}
