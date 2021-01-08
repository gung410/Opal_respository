#pragma warning disable

using System.ComponentModel;
using Xamarin.Forms;

namespace LearnerApp.Plugins.Shimmer
{
  /// <summary>
  /// <see cref="T:LearnerApp.Plugins.Shimmer.SfShimmer" /> is a loading indicator control that provides modern animations when data is being loaded.
  /// </summary>
  [DesignTimeVisible(true)]
  public class LearnerShimmer : ContentView
  {
    /// <summary>Gets or sets the content of the Shimmer.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public new static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof (Content), typeof (View), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(LearnerShimmer.OnContentChanged));
    /// <summary>
    /// Gets or sets the custom view that is used for loading view.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty CustomViewProperty = BindableProperty.Create(nameof (CustomView), typeof (View), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(LearnerShimmer.OnCustomViewChanged));
    /// <summary>Gets or sets the animation direction for Shimmer.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty WaveDirectionProperty = BindableProperty.Create(nameof (WaveDirection), typeof (WaveDirection), typeof (LearnerShimmer), (object) WaveDirection.Default, BindingMode.Default);
    /// <summary>Gets or sets the built-in shimmer view type.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof (Type), typeof (ShimmerTypes), typeof (LearnerShimmer), (object) ShimmerTypes.Persona, BindingMode.Default);
    /// <summary>Gets or sets the background color of shimmer view.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof (Color), typeof (Color), typeof (LearnerShimmer), (object) Color.FromHex("#EBEBEB"), BindingMode.Default);
    /// <summary>Gets or sets the shimmer wave color.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty WaveColorProperty = BindableProperty.Create(nameof (WaveColor), typeof (Color), typeof (LearnerShimmer), (object) Color.FromHex("#D1D1D1"), BindingMode.Default);
    /// <summary>Gets or sets the width of the wave.</summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty WaveWidthProperty = BindableProperty.Create(nameof (WaveWidth), typeof (double), typeof (LearnerShimmer), (object) 200.0, BindingMode.Default);
    /// <summary>
    /// Gets or sets the duration of the wave animation in milliseconds.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(nameof (AnimationDuration), typeof (double), typeof (LearnerShimmer), (object) 1000.0, BindingMode.Default);
    /// <summary>
    /// Gets or sets whether to load actual content of shimmer.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(nameof (IsActive), typeof (bool), typeof (LearnerShimmer), (object) true, BindingMode.Default, propertyChanged: new BindableProperty.BindingPropertyChangedDelegate(LearnerShimmer.OnIsActivePropertyChanged));
    /// <summary>
    /// Identifies the PersonaStyleProperty bindable property. This property can be used to change the style for persona template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty PersonaStyleProperty = BindableProperty.Create(nameof (PersonaStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>
    /// Identifies the ProfileStyleProperty bindable property.This property can be used to change the style for profile template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty ProfileStyleProperty = BindableProperty.Create(nameof (ProfileStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>
    /// Identifies the ArticleStyleProperty bindable property. This property can be used to change the style for article template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty ArticleStyleProperty = BindableProperty.Create(nameof (ArticleStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>
    /// Identifies the FeedStyle bindable property. This property can be used to change the style for feed template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty FeedStyleProperty = BindableProperty.Create(nameof (FeedStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>
    /// Identifies the VideoStyleProperty bindable property. This property can be used to change the style for video template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty VideoStyleProperty = BindableProperty.Create(nameof (VideoStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>
    /// Identifies the ShoppingStyleProperty bindable property. This property can be used to change the style for shopping template.
    /// </summary>
    /// <remarks>This bindable property is read-only.</remarks>
    public static readonly BindableProperty ShoppingStyleProperty = BindableProperty.Create(nameof (ShoppingStyle), typeof (LearnerTemplateStyle), typeof (LearnerShimmer), defaultBindingMode: BindingMode.Default);
    /// <summary>Gets the fade-in animation duration for content.</summary>
    private readonly uint fadeAnimationDuration = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:LearnerApp.Plugins.Shimmer.SfShimmer" /> class.
    /// </summary>
    public LearnerShimmer()
    {
      if (this.Content != null)
        this.Content.Opacity = 0.0;
      this.InitializeBuildInTemplateStyles();
    }

    /// <summary>Gets or sets the content of the Shimmer.</summary>
    public new View Content
    {
      get => (View) this.GetValue(LearnerShimmer.ContentProperty);
      set => this.SetValue(LearnerShimmer.ContentProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets the custom view that is used for loading view.
    /// </summary>
    public View CustomView
    {
      get => (View) this.GetValue(LearnerShimmer.CustomViewProperty);
      set => this.SetValue(LearnerShimmer.CustomViewProperty, (object) value);
    }

    /// <summary>Gets or sets the animation direction for Shimmer.</summary>
    public WaveDirection WaveDirection
    {
      get => (WaveDirection) this.GetValue(LearnerShimmer.WaveDirectionProperty);
      set => this.SetValue(LearnerShimmer.WaveDirectionProperty, (object) value);
    }

    /// <summary>Gets or sets the built-in shimmer view type.</summary>
    public ShimmerTypes Type
    {
      get => (ShimmerTypes) this.GetValue(LearnerShimmer.TypeProperty);
      set => this.SetValue(LearnerShimmer.TypeProperty, (object) value);
    }

    /// <summary>Gets or sets the background color of shimmer view.</summary>
    /// <value>The Shimmer view color.</value>
    public Color Color
    {
      get => (Color) this.GetValue(LearnerShimmer.ColorProperty);
      set => this.SetValue(LearnerShimmer.ColorProperty, (object) value);
    }

    /// <summary>Gets or sets the shimmer wave color.</summary>
    /// <value>The wave color.</value>
    public Color WaveColor
    {
      get => (Color) this.GetValue(LearnerShimmer.WaveColorProperty);
      set => this.SetValue(LearnerShimmer.WaveColorProperty, (object) value);
    }

    /// <summary>Gets or sets the width of the wave.</summary>
    /// <value>The wave width.</value>
    public double WaveWidth
    {
      get => (double) this.GetValue(LearnerShimmer.WaveWidthProperty);
      set => this.SetValue(LearnerShimmer.WaveWidthProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets the duration of the wave animation in milliseconds.
    /// </summary>
    /// <value>The animation duration.</value>
    public double AnimationDuration
    {
      get => (double) this.GetValue(LearnerShimmer.AnimationDurationProperty);
      set => this.SetValue(LearnerShimmer.AnimationDurationProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to load actual content of shimmer.
    /// </summary>
    /// <value> IsActive.</value>
    public bool IsActive
    {
      get => (bool) this.GetValue(LearnerShimmer.IsActiveProperty);
      set => this.SetValue(LearnerShimmer.IsActiveProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets the value of PersonaStyle in Shimmer. This property can be used to change the style for persona template.
    /// </summary>
    /// <value> Personal Style.</value>
    public LearnerTemplateStyle PersonaStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.PersonaStyleProperty);
      set => this.SetValue(LearnerShimmer.PersonaStyleProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets the value of ProfileStyle in Shimmer. This property can be used to change the style for profile template.
    /// </summary>
    /// <value> Personal Style.</value>
    public LearnerTemplateStyle ProfileStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.ProfileStyleProperty);
      set => this.SetValue(LearnerShimmer.ProfileStyleProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets the value of ArticleStyle in Shimmer. his property can be used to change the style for article template.
    /// </summary>
    /// <value> Youtube Style.</value>
    public LearnerTemplateStyle ArticleStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.ArticleStyleProperty);
      set => this.SetValue(LearnerShimmer.ArticleStyleProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets a value indicating the VideoStyle bindable property. This property can be used to change the style for video template.
    /// </summary>
    /// <value> MediaFeed Style.</value>
    public LearnerTemplateStyle VideoStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.VideoStyleProperty);
      set => this.SetValue(LearnerShimmer.VideoStyleProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets a value indicating the FeedStyleProperty bindable property. This property can be used to change the style for feed template.
    /// </summary>
    /// <value> ImageType Style.</value>
    public LearnerTemplateStyle FeedStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.FeedStyleProperty);
      set => this.SetValue(LearnerShimmer.FeedStyleProperty, (object) value);
    }

    /// <summary>
    /// Gets or sets a value indicating the ShoppingStyle bindable property. This property can be used to change the style for shopping template.
    /// </summary>
    /// <value> ImageType Style.</value>
    public LearnerTemplateStyle ShoppingStyle
    {
      get => (LearnerTemplateStyle) this.GetValue(LearnerShimmer.ShoppingStyleProperty);
      set => this.SetValue(LearnerShimmer.ShoppingStyleProperty, (object) value);
    }

    /// <summary>Invoke when content of shimmer is changed.</summary>
    /// <param name="bindable">Object.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
      if (!(bindable is LearnerShimmer sfShimmer))
        return;
      sfShimmer.OnContentChanged();
    }

    /// <summary>Invoke when custom shimmer view is changed.</summary>
    /// <param name="bindable">Object.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    private static void OnCustomViewChanged(
      BindableObject bindable,
      object oldValue,
      object newValue)
    {
      if (!(bindable is LearnerShimmer sfShimmer))
        return;
      sfShimmer.OnCustomViewChanged(newValue);
    }

    /// <summary>Invoke when IsActive of shimmer is changed.</summary>
    /// <param name="bindable">Object.</param>
    /// <param name="oldValue">Old value.</param>
    /// <param name="newValue">New value.</param>
    private static void OnIsActivePropertyChanged(
      BindableObject bindable,
      object oldValue,
      object newValue)
    {
      if (!(bindable is LearnerShimmer sfShimmer))
        return;
      sfShimmer.OnIsActivePropertyChanged();
    }

    /// <summary>Invoke when content of shimmer is changed.</summary>
    private void OnContentChanged()
    {
      if (this.Content != null)
        this.Content.Opacity = this.IsActive ? 0.0 : 1.0;
      base.Content = !this.IsActive || this.CustomView == null ? this.Content : this.CustomView;
    }

    /// <summary>Invoke when custom shimmer view is changed.</summary>
    /// <param name="newValue">New value.</param>
    private void OnCustomViewChanged(object newValue)
    {
      if (!this.IsActive)
        return;
      base.Content = newValue != null ? newValue as View : this.Content;
    }

    /// <summary>Invoke when IsActive of shimmer is changed.</summary>
    private void OnIsActivePropertyChanged()
    {
      base.Content = !this.IsActive || this.CustomView == null ? this.Content : this.CustomView;
      View content = this.Content;
      if (content == null)
        return;
      content.FadeTo(this.IsActive ? 0.0 : 1.0, this.fadeAnimationDuration, Easing.Linear);
    }

    /// <summary>Initializes built-in template styles.</summary>
    private void InitializeBuildInTemplateStyles()
    {
      this.PersonaStyle = new LearnerTemplateStyle()
      {
        CircleRadius = 50.0,
        LineSpacing = 12.0,
        LineHeight = 20.0
      };
      this.ProfileStyle = new LearnerTemplateStyle()
      {
        CircleRadius = 50.0,
        LineSpacing = 10.0,
        LineHeight = 10.0,
        Height = 240.0
      };
      this.VideoStyle = new LearnerTemplateStyle()
      {
        CircleRadius = 26.0,
        Height = 200.0,
        LineSpacing = 10.0,
        LineHeight = 13.0
      };
      this.FeedStyle = new LearnerTemplateStyle()
      {
        CircleRadius = 26.0,
        Height = 200.0,
        LineSpacing = 10.0,
        LineHeight = 13.0
      };
      this.ShoppingStyle = new LearnerTemplateStyle()
      {
        Height = 200.0,
        LineSpacing = 10.0,
        LineHeight = 13.0
      };
      this.ArticleStyle = new LearnerTemplateStyle()
      {
        SquareSize = 80.0,
        Height = 80.0,
        LineSpacing = 10.0,
        LineHeight = 10.0
      };
    }
  }
}
#pragma warning enable
