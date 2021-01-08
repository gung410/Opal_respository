#pragma warning disable

using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using LearnerApp.iOS.Renderers.Plugin;
using LearnerApp.Plugins.Shimmer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LearnerShimmer), typeof(ShimmerRenderer))]
namespace LearnerApp.iOS.Renderers.Plugin
{
  public class ShimmerRenderer : ViewRenderer
  {
    private CABasicAnimation waveAnimation;
    private bool isDisposed;

    public override CGRect Frame
    {
      get => base.Frame;
      set
      {
        base.Frame = value;
        if (this.WaveLayer == null)
          return;
        this.WaveLayer.Frame = this.Bounds;
        this.SetAnimatableValues();
        if (!(this.Element is LearnerShimmer element) || !element.IsActive)
          return;
        if (this.WaveLayer.AnimationForKey("rectPosition") == null)
          this.AddWaveAnimation();
        else
          this.RestartAnimation();
      }
    }

    internal ShimmerWaveLayer WaveLayer { get; set; }

    public static void Init()
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
      base.OnElementChanged(e);
      if (!(e?.NewElement is LearnerShimmer newElement))
        return;
      this.BackgroundColor = newElement.BackgroundColor.ToUIColor();
      this.Initialize();
      if (!newElement.IsActive)
        return;
      this.RestartAnimation();
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      LearnerShimmer element = this.Element as LearnerShimmer;
      if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
        this.BackgroundColor = element.BackgroundColor.ToUIColor();
      else if (e.PropertyName == LearnerShimmer.WaveDirectionProperty.PropertyName)
      {
        this.SetAnimatableValues();
        this.RestartAnimation();
      }
      else if (e.PropertyName == LearnerShimmer.AnimationDurationProperty.PropertyName)
      {
        this.waveAnimation.Duration = element.AnimationDuration / 1000.0;
        this.RestartAnimation();
      }
      else
      {
        if (!(e.PropertyName == LearnerShimmer.IsActiveProperty.PropertyName))
          return;
        if (element.IsActive)
          this.AddWaveAnimation();
        else
          this.RemoveWaveAnimation();
      }
    }

    public override void LayoutSubviews()
    {
      base.LayoutSubviews();
      CALayer[] sublayers = this.Layer.Sublayers;
      if (sublayers == null || sublayers.Length <= 1 || !(sublayers[0] is ShimmerWaveLayer))
        return;
      this.Layer.InsertSublayerBelow(sublayers[1], (CALayer) this.WaveLayer);
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.isDisposed & disposing)
      {
        if (this.waveAnimation != null && this.waveAnimation.Handle != IntPtr.Zero)
        {
          this.waveAnimation.Dispose();
          this.waveAnimation = (CABasicAnimation) null;
        }
        if (this.WaveLayer != null && this.WaveLayer.Handle != IntPtr.Zero)
        {
          this.WaveLayer.Dispose();
          this.WaveLayer = (ShimmerWaveLayer) null;
        }
        this.isDisposed = true;
      }
      base.Dispose(disposing);
    }

    private void Initialize()
    {
      if (!(this.Element is LearnerShimmer element))
        return;
      ShimmerWaveLayer shimmerWaveLayer = new ShimmerWaveLayer();
      shimmerWaveLayer.NeedsDisplayOnBoundsChange = true;
      shimmerWaveLayer.ContentsScale = UIScreen.MainScreen.Scale;
      shimmerWaveLayer.MasksToBounds = true;
      shimmerWaveLayer.AnchorPoint = new CGPoint(0.0f, 0.0f);
      shimmerWaveLayer.BackgroundColor = UIColor.Clear.CGColor;
      shimmerWaveLayer.Opacity = 0.0f;
      shimmerWaveLayer.Frame = this.Bounds;
      shimmerWaveLayer.FormsShimmer = element;
      this.WaveLayer = shimmerWaveLayer;
      this.waveAnimation = CABasicAnimation.FromKeyPath("rectPosition");
      this.waveAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
      this.waveAnimation.RemovedOnCompletion = false;
      this.waveAnimation.RepeatCount = float.PositiveInfinity;
      this.waveAnimation.Duration = element.AnimationDuration / 1000.0;
      this.SetAnimatableValues();
      this.Layer.AddSublayer((CALayer) this.WaveLayer);
    }

    private void SetAnimatableValues()
    {
      if (this.waveAnimation == null || !(this.waveAnimation.Handle != IntPtr.Zero) || !(this.Element is LearnerShimmer element))
        return;
      switch (element.WaveDirection)
      {
        case WaveDirection.Default:
        case WaveDirection.LeftToRight:
          this.waveAnimation.From = NSObject.FromObject((object) new CGPoint((double) this.WaveLayer.Bounds.X - element.Width / 2.0, (double) this.WaveLayer.Bounds.Y));
          this.waveAnimation.To = NSObject.FromObject((object) new CGPoint(this.Bounds.Width, this.WaveLayer.Bounds.Y));
          break;
        case WaveDirection.TopToBottom:
          this.waveAnimation.From = NSObject.FromObject((object) new CGPoint(this.WaveLayer.Bounds.X, this.WaveLayer.Bounds.Y));
          this.waveAnimation.To = NSObject.FromObject((object) new CGPoint(this.WaveLayer.Bounds.X, this.Bounds.Height));
          break;
        case WaveDirection.RightToLeft:
          CABasicAnimation waveAnimation1 = this.waveAnimation;
          double x1 = (double) this.Bounds.Width + element.Width / 2.0;
          CGRect bounds = this.WaveLayer.Bounds;
          double y1 = (double) bounds.Y;
          NSObject nsObject1 = NSObject.FromObject((object) new CGPoint(x1, y1));
          waveAnimation1.From = nsObject1;
          CABasicAnimation waveAnimation2 = this.waveAnimation;
          bounds = this.WaveLayer.Bounds;
          nfloat x2 = bounds.X;
          bounds = this.WaveLayer.Bounds;
          nfloat y2 = bounds.Y;
          NSObject nsObject2 = NSObject.FromObject((object) new CGPoint(x2, y2));
          waveAnimation2.To = nsObject2;
          break;
        case WaveDirection.BottomToTop:
          this.waveAnimation.From = NSObject.FromObject((object) new CGPoint(this.WaveLayer.Bounds.X, this.Bounds.Height));
          this.waveAnimation.To = NSObject.FromObject((object) new CGPoint(this.WaveLayer.Bounds.X, this.WaveLayer.Bounds.Y));
          break;
      }
    }

    private void AddWaveAnimation()
    {
      this.WaveLayer.Opacity = 1f;
      CABasicAnimation waveAnimation = this.waveAnimation;
      if ((waveAnimation != null ? (waveAnimation.Duration == 0.0 ? 1 : 0) : 0) != 0)
        this.WaveLayer.SetNeedsDisplay();
      else
        this.WaveLayer.AddAnimation((CAAnimation) this.waveAnimation, "rectPosition");
    }

    private void RemoveWaveAnimation()
    {
      this.WaveLayer.RemoveAnimation("rectPosition");
      this.WaveLayer.Opacity = 0.0f;
    }

    private void RestartAnimation()
    {
      if (!(this.Element as LearnerShimmer).IsActive)
        return;
      this.RemoveWaveAnimation();
      this.AddWaveAnimation();
    }
  }
}
