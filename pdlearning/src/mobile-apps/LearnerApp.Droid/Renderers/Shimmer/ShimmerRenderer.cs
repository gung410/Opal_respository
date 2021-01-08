#pragma warning disable

using System;
using System.ComponentModel;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using LearnerApp.Droid.Renderers.Shimmer;
using LearnerApp.Plugins.Shimmer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LearnerShimmer), typeof(ShimmerRenderer))]
namespace LearnerApp.Droid.Renderers.Shimmer
{
  public class ShimmerRenderer : ViewRenderer
  {
    private int centerY;
    private int templateX;
    private int templateWidth;
    private int templateHeight;
    private int columnSpacing;
    private Paint wavePaint;
    private Matrix shimmerMatrix;
    private ValueAnimator animator;
    private Paint shimmerPaint;
    private float density;
    private Shader shader;

    private void DrawShimmerView(Canvas canvas)
    {
      Android.Views.View childAt = this.GetChildAt(0);
      if (childAt != null)
      {
        this.centerY = (int) childAt.GetY() + childAt.Height / 2;
        this.templateX = (int) childAt.GetX();
        this.templateWidth = childAt.Width;
        this.templateHeight = childAt.Height;
      }
      else
      {
        this.centerY = this.Height / 2;
        this.templateX = 0;
        this.templateWidth = this.Width;
        this.templateHeight = this.Height;
      }
      this.columnSpacing = 15 * (int) this.density;
      switch ((this.Element as LearnerShimmer).Type)
      {
        case ShimmerTypes.Persona:
          this.DrawPersona(canvas);
          break;
        case ShimmerTypes.Profile:
          this.DrawProfile(canvas);
          break;
        case ShimmerTypes.Article:
          this.DrawArticle(canvas);
          break;
        case ShimmerTypes.Video:
          this.DrawVideo(canvas);
          break;
        case ShimmerTypes.Feed:
          this.DrawFeed(canvas);
          break;
        case ShimmerTypes.Shopping:
          this.DrawShopping(canvas);
          break;
      }
    }

    private void DrawPersona(Canvas canvas)
    {
        LearnerTemplateStyle personaStyle = (this.Element as LearnerShimmer).PersonaStyle;
      int num1 = (int) (personaStyle.CircleRadius * (double) this.density);
      int num2 = (int) (personaStyle.LineSpacing * (double) this.density);
      int num3 = (int) (personaStyle.LineHeight * (double) this.density);
      int num4 = (int) (personaStyle.Width * (double) this.density);
      if (num4 < 0)
        num4 = this.templateWidth;
      int num5 = (int) (personaStyle.Height * (double) this.density);
      int num6 = num5 > 0 ? num5 / 2 : this.templateHeight / 2;
      int num7 = num6 > 0 ? (num1 <= num6 ? num1 : num6) : num1;
      int left = this.templateX + num7 * 2 + this.columnSpacing;
      canvas.DrawCircle((float) (this.templateX + num7), (float) this.centerY, (float) num7, this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY - num7 + num2, left + (num4 - num7 * 2 - this.columnSpacing) * 3 / 4, this.centerY - num7 + num2 + num3), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY - num7 + num2 + num3 + num2, left + (num4 - num7 * 2 - this.columnSpacing) / 2, this.centerY - num7 + 2 * num2 + num3 * 2), this.shimmerPaint);
    }

    private void DrawVideo(Canvas canvas)
    {
      LearnerTemplateStyle videoStyle = (this.Element as LearnerShimmer).VideoStyle;
      int num1 = (int) (videoStyle.CircleRadius * (double) this.density);
      int num2 = (int) (videoStyle.LineSpacing * (double) this.density);
      int num3 = (int) (videoStyle.LineHeight * (double) this.density);
      int num4 = (int) (videoStyle.Width * (double) this.density);
      if (num4 < 0)
        num4 = this.templateWidth;
      int num5 = (int) (videoStyle.Height * (double) this.density);
      int num6 = num5 >= 0 ? num5 / 2 : this.templateHeight / 2;
      int left = this.templateX + num1 * 2 + this.columnSpacing;
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY - num6, this.templateX + num4, this.centerY + num6 - num1 * 2 - num2), this.shimmerPaint);
      canvas.DrawCircle((float) (this.templateX + num1), (float) (this.centerY + num6 - num1), (float) num1, this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY + num6 - num1 * 2 + num2, this.templateX + num4, this.centerY + num6 - num1 * 2 + num2 + num3), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY + num6 - num1 * 2 + 2 * num2 + num3, this.templateX + num4, this.centerY + num6 - num1 * 2 + num2 + num3 * 2 + num2), this.shimmerPaint);
    }

    private void DrawFeed(Canvas canvas)
    {
      LearnerTemplateStyle feedStyle = (this.Element as LearnerShimmer).FeedStyle;
      int num1 = (int) (feedStyle.CircleRadius * (double) this.density);
      int num2 = (int) (feedStyle.LineSpacing * (double) this.density);
      int num3 = (int) (feedStyle.LineHeight * (double) this.density);
      int num4 = (int) (feedStyle.Width * (double) this.density);
      if (num4 < 0)
        num4 = this.templateWidth;
      int num5 = (int) (feedStyle.Height * (double) this.density);
      int num6 = num5 >= 0 ? num5 / 2 : this.templateHeight / 2;
      canvas.DrawCircle((float) (this.templateX + num1), (float) (this.centerY - num6 + num1), (float) num1, this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX + num1 * 2 + this.columnSpacing, this.centerY - num6 + num2, this.templateX + num4, this.centerY - num6 + num2 + num3), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX + num1 * 2 + this.columnSpacing, this.centerY - num6 + 2 * num2 + num3, this.templateX + num4, this.centerY - num6 + num2 + num3 * 2 + num2), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY - num6 + num1 * 2 + num2, this.templateX + num4, this.centerY + num6 - (int) ((double) num3 * 0.6 * 2.0) - (int) ((double) num2 * 0.6 * 2.0 * 1.5)), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num6 - (int) ((double) num3 * 0.6 * 2.0) - (int) ((double) num2 * 0.6), this.templateX + num4, this.centerY + num6 - (int) ((double) num3 * 0.6) - (int) ((double) num2 * 0.6)), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num6 - (int) ((double) num3 * 0.6), this.templateX + num4, this.centerY + num6), this.shimmerPaint);
    }

    private void DrawShopping(Canvas canvas)
    {
      LearnerTemplateStyle shoppingStyle = (this.Element as LearnerShimmer).ShoppingStyle;
      int num1 = (int) (shoppingStyle.LineSpacing * (double) this.density);
      int num2 = (int) (shoppingStyle.LineHeight * (double) this.density);
      int num3 = (int) (shoppingStyle.Width * (double) this.density);
      if (num3 < 0)
        num3 = this.templateWidth;
      int num4 = (int) (shoppingStyle.Height * (double) this.density);
      int num5 = num4 >= 0 ? num4 / 2 : this.templateHeight / 2;
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY - num5, this.templateX + num3, this.centerY + num5 - (int) ((double) num2 * 0.6 * 2.0) - (int) ((double) num1 * 0.6 * 2.0 * 1.5)), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num5 - (int) ((double) num2 * 0.6 * 2.0) - (int) ((double) num1 * 0.6), this.templateX + num3, this.centerY + num5 - (int) ((double) num2 * 0.6) - (int) ((double) num1 * 0.6)), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num5 - (int) ((double) num2 * 0.6), this.templateX + num3, this.centerY + num5), this.shimmerPaint);
    }

    private void DrawProfile(Canvas canvas)
    {
      LearnerTemplateStyle profileStyle = (this.Element as LearnerShimmer).ProfileStyle;
      int num1 = (int) (profileStyle.CircleRadius * (double) this.density);
      int num2 = (int) (profileStyle.LineSpacing * (double) this.density);
      int num3 = (int) (profileStyle.LineHeight * (double) this.density);
      int num4 = (int) (profileStyle.Width * (double) this.density);
      if (num4 < 0)
        num4 = this.templateWidth;
      int num5 = (int) (profileStyle.Height * (double) this.density);
      int num6 = num5 >= 0 ? num5 / 2 : this.templateHeight / 2;
      int num7 = num4 / 2;
      canvas.DrawCircle((float) (this.templateX + num7), (float) (this.centerY - num6 + num1), (float) num1, this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX + num7 - num4 / 5, this.centerY + num2, this.templateX + num7 + num4 / 5, this.centerY + num2 + num3 * 2), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX + num7 - num4 * 3 / 10, this.centerY + num2 * 2 + num3 * 2, this.templateX + num7 + num4 * 3 / 10, this.centerY + num2 * 2 + num3 * 4), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num6 - num3 * 2 - num2, this.templateX + num4, this.centerY + num6 - num3 - num2), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY + num6 - num3, this.templateX + num4, this.centerY + num6), this.shimmerPaint);
    }

    private void DrawArticle(Canvas canvas)
    {
      LearnerTemplateStyle articleStyle = (this.Element as LearnerShimmer).ArticleStyle;
      int num1 = (int) (articleStyle.LineSpacing * (double) this.density);
      int num2 = (int) (articleStyle.LineHeight * (double) this.density);
      int num3 = (int) (articleStyle.SquareSize * (double) this.density);
      int num4 = (int) (articleStyle.Width * (double) this.density);
      if (num4 < 0)
        num4 = this.templateWidth;
      int num5 = (int) (articleStyle.Height * (double) this.density);
      int num6 = num5 >= 0 ? num5 / 2 : this.templateHeight / 2;
      int left = this.templateX + num3 + this.columnSpacing;
      canvas.DrawRect(new Android.Graphics.Rect(this.templateX, this.centerY - num6, this.templateX + num3, this.centerY + num6), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY - num6, this.templateX + num4, this.centerY - num6 + num2), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY - num6 + num2 + num1, (num4 - num3 - this.columnSpacing) * 4 / 5 + left, this.centerY - num6 + num2 * 2 + num1), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY + num1, (num4 - num3 - this.columnSpacing) * 2 / 5 + left, this.centerY + num1 + num2), this.shimmerPaint);
      canvas.DrawRect(new Android.Graphics.Rect(left, this.centerY + num2 + num1 * 2, (num4 - num3 - this.columnSpacing) * 3 / 10 + left, this.centerY + num2 * 2 + num1 * 2), this.shimmerPaint);
    }

    public ShimmerRenderer(Context context)
      : base(context)
    {
      this.wavePaint = new Paint();
      this.shimmerMatrix = new Matrix();
      this.shimmerPaint = new Paint();
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
    {
      base.OnElementChanged(e);
      if (!(e?.NewElement is LearnerShimmer newElement))
        return;
      this.density = this.Context.Resources.DisplayMetrics.Density;
      this.SetWillNotDraw(false);
      this.wavePaint.AntiAlias = true;
      this.wavePaint.SetXfermode((Xfermode) new PorterDuffXfermode(PorterDuff.Mode.SrcAtop));
      this.shimmerPaint.AntiAlias = true;
      this.shimmerPaint.Color = newElement.Color.ToAndroid();
      this.SetBackgroundColor(Android.Graphics.Color.Transparent);
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      LearnerShimmer element = this.Element as LearnerShimmer;
      if (e.PropertyName == "Color" || e.PropertyName == "WaveColor" || (e.PropertyName == "WaveWidth" || e.PropertyName == "WaveDirection"))
      {
        this.AddWaveShader();
        this.Invalidate();
      }
      else if (e.PropertyName == "AnimationDuration")
      {
        this.AddWaveAnimator();
        this.Invalidate();
      }
      else if (e.PropertyName == "Type")
        this.Invalidate();
      else if (e.PropertyName == "IsActive")
      {
        if (element.IsActive)
          this.UpdateShimmerWave();
        else
          this.animator?.End();
      }
      else
      {
        if (!(e.PropertyName == "BackgroundColor"))
          return;
        this.SetBackgroundColor(Android.Graphics.Color.Transparent);
      }
    }

    protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
    {
      base.OnLayout(changed, left, top, right, bottom);
      if (!(this.Element as LearnerShimmer).IsActive)
        return;
      this.UpdateShimmerWave();
    }

    protected override void DispatchDraw(Canvas canvas)
    {
      LearnerShimmer element = this.Element as LearnerShimmer;
      if (!element.IsActive || this.Height == 0 || this.Width == 0)
      {
        base.DispatchDraw(canvas);
      }
      else
      {
        if (element.CustomView == null)
          this.DrawShimmerView(canvas);
        base.DispatchDraw(canvas);
        this.DrawShimmerWave(canvas);
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.wavePaint != null && this.wavePaint.Handle != IntPtr.Zero)
        {
          this.wavePaint.Dispose();
          this.wavePaint = (Paint) null;
        }
        if (this.shimmerPaint != null && this.shimmerPaint.Handle != IntPtr.Zero)
        {
          this.shimmerPaint.Dispose();
          this.shimmerPaint = (Paint) null;
        }
        if (this.animator != null && this.animator.Handle != IntPtr.Zero)
        {
          this.animator.End();
          this.animator.Update -= new EventHandler<ValueAnimator.AnimatorUpdateEventArgs>(this.OnWaveAnimatorUpdate);
          this.animator.Dispose();
          this.animator = (ValueAnimator) null;
        }
        if (this.shimmerMatrix != null && this.shimmerMatrix.Handle != IntPtr.Zero)
        {
          this.shimmerMatrix.Dispose();
          this.shimmerMatrix = (Matrix) null;
        }
        if (this.shader != null && this.shader.Handle != IntPtr.Zero)
        {
          this.shader.Dispose();
          this.shader = (Shader) null;
        }
      }
      base.Dispose(disposing);
    }

    private void DrawShimmerWave(Canvas canvas)
    {
      float num1 = (float) Math.Tan(Math.PI / 9.0);
      float num2 = (float) this.Height + num1 * (float) this.Width;
      float num3 = (float) this.Width + num1 * (float) this.Height;
      float percent = this.animator != null ? this.animator.AnimatedFraction : 0.0f;
      this.shimmerMatrix?.Reset();
      LearnerShimmer element = this.Element as LearnerShimmer;
      float dx;
      float dy;
      switch (element.WaveDirection)
      {
        case WaveDirection.TopToBottom:
          dx = 0.0f;
          dy = this.Offset(-num2, num2, percent);
          break;
        case WaveDirection.RightToLeft:
          dx = this.Offset(num3, -num3, percent);
          dy = 0.0f;
          break;
        case WaveDirection.BottomToTop:
          dx = 0.0f;
          dy = this.Offset(num2, -num2, percent);
          break;
        default:
          dx = this.Offset(-num3, num3, percent);
          dy = 0.0f;
          if (element.WaveDirection == WaveDirection.Default)
          {
            this.shimmerMatrix.SetRotate(25f, (float) this.Width / 2f, (float) this.Height / 2f);
            break;
          }
          break;
      }
      this.shimmerMatrix.PostTranslate(dx, dy);
      this.wavePaint.Shader?.SetLocalMatrix(this.shimmerMatrix);
      canvas.DrawRect(new Android.Graphics.Rect(0, 0, this.Width, this.Height), this.wavePaint);
    }

    private float Offset(float start, float end, float percent) => start + (end - start) * percent;

    public void AddWaveShader()
    {
      LearnerShimmer element = this.Element as LearnerShimmer;
      int num1 = element.WaveDirection == WaveDirection.BottomToTop ? 1 : (element.WaveDirection == WaveDirection.TopToBottom ? 1 : 0);
      int num2 = num1 != 0 ? 0 : this.Width;
      int num3 = num1 != 0 ? this.Height : 0;
      int android1 = (int) element.WaveColor.ToAndroid();
      int android2 = (int) element.Color.ToAndroid();
      float num4 = (float) element.WaveWidth * this.density / (float) this.Width;
      float num5 = (double) num4 < 0.0 ? 0.0f : ((double) num4 > 1.0 ? 1f : num4);
      float[] positions = new float[3]
      {
        (float) (0.5 - (double) num5 / 2.0),
        0.5f,
        (float) (0.5 + (double) num5 / 2.0)
      };
      this.shader = (Shader) new LinearGradient(0.0f, 0.0f, (float) num2, (float) num3, new int[3]
      {
        android2,
        android1,
        android2
      }, positions, Shader.TileMode.Clamp);
      this.wavePaint.SetShader(this.shader);
    }

    public void AddWaveAnimator()
    {
      if (this.animator == null)
      {
        this.animator = ValueAnimator.OfFloat(0.0f, 1f);
        this.animator.RepeatCount = -1;
        this.animator.SetInterpolator((ITimeInterpolator) new LinearInterpolator());
        this.animator.Update += new EventHandler<ValueAnimator.AnimatorUpdateEventArgs>(this.OnWaveAnimatorUpdate);
      }
      else
        this.animator.End();
      this.animator.SetDuration((long) (int) (this.Element as LearnerShimmer).AnimationDuration);
      this.animator.Start();
    }

    private void OnWaveAnimatorUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e) => this.Invalidate();

    private void UpdateShimmerWave()
    {
      this.SetLayerType(LayerType.Hardware, this.wavePaint);
      this.AddWaveShader();
      this.AddWaveAnimator();
    }
  }
}
