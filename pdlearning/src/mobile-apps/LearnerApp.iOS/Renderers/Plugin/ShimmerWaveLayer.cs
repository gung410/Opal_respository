#pragma warning disable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using LearnerApp.Plugins.Shimmer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace LearnerApp.iOS.Renderers.Plugin
{
  internal class ShimmerWaveLayer : CALayer
  {
    private readonly float columnSpacing = 15f;
    private float centerY;
    private float templateX;
    private float templateWidth;
    private float templateHeight;
    private CGRect waveRect;
    private CGGradient gradient;
    private CGPath roundedPath;
    private ObservableCollection<ShimmerWaveLayer> clonedLayerObjects = new ObservableCollection<ShimmerWaveLayer>();

    private void DrawBuiltInView(CGContext context)
    {
      CALayer sublayer = this.SuperLayer.Sublayers[0];
      if (sublayer != null)
      {
        CGRect frame = sublayer.Frame;
        this.centerY = (float) (frame.Y + frame.Height / (nfloat) 2);
        this.templateX = (float) frame.X;
        this.templateWidth = (float) frame.Width;
        this.templateHeight = (float) frame.Height;
      }
      else
      {
        this.centerY = (float) this.Bounds.Height / 2f;
        this.templateX = (float) this.Bounds.X;
        this.templateWidth = (float) this.Bounds.Width;
        this.templateHeight = (float) this.Bounds.Height;
      }
      context.SetStrokeColor(UIColor.Clear.CGColor);
      context.SetFillColor(this.FormsShimmer.Color.ToCGColor());
      switch (this.FormsShimmer.Type)
      {
        case ShimmerTypes.Persona:
          this.DrawPersona(context);
          break;
        case ShimmerTypes.Profile:
          this.DrawProfile(context);
          break;
        case ShimmerTypes.Article:
          this.DrawArticle(context);
          break;
        case ShimmerTypes.Video:
          this.DrawVideo(context);
          break;
        case ShimmerTypes.Feed:
          this.DrawFeed(context);
          break;
        case ShimmerTypes.Shopping:
          this.DrawShopping(context);
          break;
      }
      context.DrawPath(CGPathDrawingMode.FillStroke);
    }

    private void DrawPersona(CGContext context)
    {
      LearnerTemplateStyle personaStyle = this.FormsShimmer.PersonaStyle;
      float circleRadius = (float) personaStyle.CircleRadius;
      float lineSpacing = (float) personaStyle.LineSpacing;
      float lineHeight = (float) personaStyle.LineHeight;
      float num1 = personaStyle.Width >= 0.0 ? (float) personaStyle.Width : this.templateWidth;
      float num2 = (personaStyle.Height >= 0.0 ? (float) personaStyle.Height : this.templateHeight) / 2f;
      float num3 = (double) num2 > 0.0 ? ((double) circleRadius <= (double) num2 ? circleRadius : num2) : circleRadius;
      float num4 = this.templateX + num3 * 2f + this.columnSpacing;
      context.AddEllipseInRect(new CGRect(this.templateX, this.centerY - num3, num3 * 2f, num3 * 2f));
      context.AddRect(new CGRect((double) num4, (double) this.centerY - (double) num3 + (double) lineSpacing, ((double) num1 - (double) this.columnSpacing - (double) num3 * 2.0) * 0.8, (double) lineHeight));
      context.AddRect(new CGRect((double) num4, (double) this.centerY - (double) num3 + (double) lineSpacing + (double) lineHeight + (double) lineSpacing, ((double) num1 - (double) this.columnSpacing - (double) num3 * 2.0) * 0.5, (double) lineHeight));
    }

    private void DrawVideo(CGContext context)
    {
      LearnerTemplateStyle videoStyle = this.FormsShimmer.VideoStyle;
      float circleRadius = (float) videoStyle.CircleRadius;
      float lineSpacing = (float) videoStyle.LineSpacing;
      float lineHeight = (float) videoStyle.LineHeight;
      float width = videoStyle.Width >= 0.0 ? (float) videoStyle.Width : this.templateWidth;
      float num1 = (videoStyle.Height >= 0.0 ? (float) videoStyle.Height : this.templateHeight) / 2f;
      float num2 = this.templateX + circleRadius * 2f + this.columnSpacing;
      context.AddRect(new CGRect(this.templateX, this.centerY - num1, width, (float) ((double) num1 * 2.0 - (double) circleRadius * 2.0) - lineSpacing));
      context.AddEllipseInRect(new CGRect(this.templateX, (float) ((double) this.centerY + (double) num1 - (double) circleRadius * 2.0), circleRadius * 2f, circleRadius * 2f));
      context.AddRect(new CGRect((double) num2, (double) this.centerY + (double) num1 - (double) circleRadius * 2.0 + (double) lineSpacing * 0.5, (double) width - (double) this.columnSpacing - (double) circleRadius * 2.0, (double) lineHeight));
      context.AddRect(new CGRect((double) num2, (double) this.centerY + (double) num1 - (double) circleRadius * 2.0 + (double) lineSpacing * 1.5 + (double) lineHeight, (double) width - (double) this.columnSpacing - (double) circleRadius * 2.0, (double) lineHeight));
    }

    private void DrawFeed(CGContext context)
    {
      LearnerTemplateStyle feedStyle = this.FormsShimmer.FeedStyle;
      float circleRadius = (float) feedStyle.CircleRadius;
      float lineSpacing = (float) feedStyle.LineSpacing;
      float lineHeight = (float) feedStyle.LineHeight;
      float num1 = feedStyle.Width >= 0.0 ? (float) feedStyle.Width : this.templateWidth;
      float num2 = (feedStyle.Height >= 0.0 ? (float) feedStyle.Height : this.templateHeight) / 2f;
      float num3 = this.templateX + circleRadius * 2f + this.columnSpacing;
      context.AddEllipseInRect(new CGRect(this.templateX, this.centerY - num2, circleRadius * 2f, circleRadius * 2f));
      context.AddRect(new CGRect((double) num3, (double) this.centerY - (double) num2 + (double) lineSpacing * 0.5, (double) num1 - (double) this.columnSpacing - (double) circleRadius * 2.0, (double) lineHeight));
      context.AddRect(new CGRect((double) num3, (double) this.centerY - (double) num2 + (double) lineSpacing * 1.5 + (double) lineHeight, ((double) num1 - (double) this.columnSpacing - (double) circleRadius * 2.0) * 0.5, (double) lineHeight));
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY - (double) num2 + (double) circleRadius * 2.0 + (double) lineSpacing, (double) num1, (double) num2 * 2.0 - (double) lineHeight * 0.6 * 2.0 - (double) lineSpacing * 0.6 * 2.0 * 1.5 - (double) circleRadius * 2.0 - (double) lineSpacing));
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY + (double) num2 - (double) lineHeight * 0.6 * 2.0 - (double) lineSpacing * 0.6, (double) num1, (double) lineHeight * 0.6));
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY + (double) num2 - (double) lineHeight * 0.6, (double) num1, (double) lineHeight * 0.6));
    }

    private void DrawShopping(CGContext context)
    {
      LearnerTemplateStyle shoppingStyle = this.FormsShimmer.ShoppingStyle;
      float lineSpacing = (float) shoppingStyle.LineSpacing;
      float lineHeight = (float) shoppingStyle.LineHeight;
      float num1 = shoppingStyle.Width >= 0.0 ? (float) shoppingStyle.Width : this.templateWidth;
      float num2 = (shoppingStyle.Height >= 0.0 ? (float) shoppingStyle.Height : this.templateHeight) / 2f;
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY - (double) num2, (double) num1, (double) num2 * 2.0 - (double) lineHeight * 0.6 * 2.0 - (double) lineSpacing * 0.6 * 2.0 * 1.5));
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY + (double) num2 - (double) lineHeight * 0.6 * 2.0 - (double) lineSpacing * 0.6, (double) num1, (double) lineHeight * 0.6));
      context.AddRect(new CGRect((double) this.templateX, (double) this.centerY + (double) num2 - (double) lineHeight * 0.6, (double) num1, (double) lineHeight * 0.6));
    }

    private void DrawArticle(CGContext context)
    {
      LearnerTemplateStyle articleStyle = this.FormsShimmer.ArticleStyle;
      float lineSpacing = (float) articleStyle.LineSpacing;
      float lineHeight = (float) articleStyle.LineHeight;
      float squareSize = (float) articleStyle.SquareSize;
      float num1 = articleStyle.Width >= 0.0 ? (float) articleStyle.Width : this.templateWidth;
      float num2 = (articleStyle.Height >= 0.0 ? (float) articleStyle.Height : this.templateHeight) / 2f;
      float x = this.templateX + squareSize + this.columnSpacing;
      context.AddRect(new CGRect(this.templateX, this.centerY - num2, squareSize, num2 * 2f));
      context.AddRect(new CGRect(x, this.centerY - num2, num1 - this.columnSpacing - squareSize, lineHeight));
      context.AddRect(new CGRect((double) x, (double) this.centerY - (double) num2 + (double) lineHeight + (double) lineSpacing, ((double) num1 - (double) this.columnSpacing - (double) squareSize) * 0.8, (double) lineHeight));
      context.AddRect(new CGRect((double) x, (double) this.centerY + (double) lineSpacing, ((double) num1 - (double) this.columnSpacing - (double) squareSize) * 0.4, (double) lineHeight));
      context.AddRect(new CGRect((double) x, (double) this.centerY + (double) lineHeight + (double) lineSpacing * 2.0, ((double) num1 - (double) this.columnSpacing - (double) squareSize) * 0.3, (double) lineHeight));
    }

    private void DrawProfile(CGContext context)
    {
      LearnerTemplateStyle profileStyle = this.FormsShimmer.ProfileStyle;
      float circleRadius = (float) profileStyle.CircleRadius;
      float lineSpacing = (float) profileStyle.LineSpacing;
      float lineHeight = (float) profileStyle.LineHeight;
      float width = profileStyle.Width >= 0.0 ? (float) profileStyle.Width : this.templateWidth;
      float num1 = (profileStyle.Height >= 0.0 ? (float) profileStyle.Height : this.templateHeight) / 2f;
      double num2 = (double) width / 2.0;
      float num3 = width / 2f;
      context.AddEllipseInRect(new CGRect(this.templateX + num3 - circleRadius, this.centerY - num1, circleRadius * 2f, circleRadius * 2f));
      context.AddRect(new CGRect((double) this.templateX + (double) num3 - (double) width * 0.2, (double) this.centerY + (double) lineSpacing, (double) width * 0.2 * 2.0, (double) lineHeight * 2.0));
      context.AddRect(new CGRect((double) this.templateX + (double) num3 - (double) width * 0.3, (double) this.centerY + (double) lineSpacing * 2.0 + (double) lineHeight * 2.0, (double) width * 0.3 * 2.0, (double) lineHeight * 2.0));
      context.AddRect(new CGRect(this.templateX, (float) ((double) this.centerY + (double) num1 - (double) lineHeight * 2.0) - lineSpacing, width, lineHeight));
      context.AddRect(new CGRect(this.templateX, this.centerY + num1 - lineHeight, width, lineHeight));
    }

    internal ShimmerWaveLayer()
    {
    }

    [Export("initWithLayer:")]
    internal ShimmerWaveLayer(CALayer layer)
      : base(layer)
    {
    }

    internal LearnerShimmer FormsShimmer { get; set; }

    [Export("rectPosition")]
    internal CGPoint RectPosition { get; set; }

    [Export("needsDisplayForKey:")]
    public static bool NeedsDisplayForKey(NSString key) => key.ToString() == "rectPosition" || CALayer.NeedsDisplayForKey((string) key);

    public override void Clone(CALayer layer)
    {
      this.FormsShimmer = ((ShimmerWaveLayer) layer).FormsShimmer;
      ((ShimmerWaveLayer) layer).clonedLayerObjects.Add(this);
      base.Clone(layer);
    }

    public override void DrawInContext(CGContext context)
    {
      base.DrawInContext(context);
      if (this.FormsShimmer.CustomView == null)
        this.DrawBuiltInView(context);
      else
        this.DrawCustomView(context);
      if (this.FormsShimmer.AnimationDuration <= 0.0 || this.FormsShimmer.WaveWidth <= 0.0)
        return;
      this.DrawShimmerWave(context);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.gradient != null && this.gradient.Handle != IntPtr.Zero)
      {
        this.gradient.Dispose();
        this.gradient = (CGGradient) null;
      }
      if (this.roundedPath != (CGPath) null && this.roundedPath.Handle != IntPtr.Zero)
      {
        this.roundedPath.Dispose();
        this.roundedPath = (CGPath) null;
      }
      if (this.clonedLayerObjects != null && this.clonedLayerObjects.Count > 0)
      {
        for (int index = 0; index < this.clonedLayerObjects.Count; ++index)
        {
          this.clonedLayerObjects[index].RemoveAllAnimations();
          this.clonedLayerObjects[index].Dispose();
          this.clonedLayerObjects[index] = (ShimmerWaveLayer) null;
        }
        this.clonedLayerObjects.Clear();
      }
      base.Dispose(disposing);
    }

    private void DrawCustomView(CGContext context)
    {
      if (this.FormsShimmer == null || this.FormsShimmer.CustomView == null)
        return;
      context.SetStrokeColor(UIColor.Clear.CGColor);
      context.SetFillColor(this.FormsShimmer.Color.ToCGColor());
      foreach (ShimmerViewRenderer shimmerViewRenderer in this.GetShimmerViewRenderers(Xamarin.Forms.Platform.iOS.Platform.GetRenderer((VisualElement) this.FormsShimmer.CustomView) as UIView))
      {
        if (shimmerViewRenderer.Element == null || !(shimmerViewRenderer.Element is ShimmerView element))
          return;
        nfloat nfloat1 = (nfloat) element.CornerRadius;
        (double x2, double y2) = this.GetPosition((VisualElement) element);
        CGRect frame = shimmerViewRenderer.Frame;
        CGRect rect = new CGRect(x2, y2, (double) frame.Width, (double) frame.Height);
        if (nfloat1 > (nfloat) 0)
        {
          if (this.roundedPath == (CGPath) null)
            this.roundedPath = new CGPath();
          nfloat nfloat2 = (nfloat) Math.Min((double) (frame.Width / (nfloat) 2), (double) (frame.Height / (nfloat) 2));
          if (nfloat1 > nfloat2)
            nfloat1 = nfloat2;
          this.roundedPath.AddRoundedRect(rect, nfloat1, nfloat1);
          context.AddPath(this.roundedPath);
        }
        else
          context.AddRect(rect);
      }
      context.DrawPath(CGPathDrawingMode.FillStroke);
    }

    private void DrawShimmerWave(CGContext context)
    {
      nfloat x = this.RectPosition.X;
      nfloat y = this.RectPosition.Y;
      CGPoint startPoint = CGPoint.Empty;
      CGPoint endPoint = CGPoint.Empty;
      switch (this.FormsShimmer.WaveDirection)
      {
        case WaveDirection.Default:
          this.waveRect = new CGRect((double) x, (double) y, this.FormsShimmer.WaveWidth, (double) this.Bounds.Height);
          startPoint = new CGPoint(this.waveRect.X, this.waveRect.Y);
          endPoint = new CGPoint(this.waveRect.Right, this.waveRect.Y);
          if (this.FormsShimmer.WaveDirection == WaveDirection.Default)
          {
            context.TranslateCTM(this.waveRect.X, this.waveRect.Y);
            context.RotateCTM((nfloat) (5f * (float) Math.PI / 36f));
            break;
          }
          break;
        case WaveDirection.LeftToRight:
          this.waveRect = new CGRect((double) x, (double) y, this.FormsShimmer.WaveWidth, (double) this.Bounds.Height);
          startPoint = new CGPoint(this.waveRect.X, this.waveRect.Y);
          endPoint = new CGPoint(this.waveRect.Right, this.waveRect.Y);
          break;
        case WaveDirection.TopToBottom:
          this.waveRect = new CGRect((double) x, (double) y, (double) this.Bounds.Width, this.FormsShimmer.WaveWidth);
          startPoint = new CGPoint(this.waveRect.X, this.waveRect.Y);
          endPoint = new CGPoint(this.waveRect.X, this.waveRect.Bottom);
          break;
        case WaveDirection.RightToLeft:
          this.waveRect = new CGRect((double) x, (double) y, -this.FormsShimmer.WaveWidth, (double) this.Bounds.Height);
          startPoint = new CGPoint(this.waveRect.Right, this.waveRect.Y);
          endPoint = new CGPoint(this.waveRect.X, this.waveRect.Y);
          break;
        case WaveDirection.BottomToTop:
          this.waveRect = new CGRect((double) x, (double) y, (double) this.Bounds.Width, -this.FormsShimmer.WaveWidth);
          startPoint = new CGPoint(this.waveRect.X, this.waveRect.Bottom);
          endPoint = new CGPoint(this.waveRect.X, this.waveRect.Y);
          break;
      }
      if (this.gradient == null)
      {
        CGColor cgColor1 = this.FormsShimmer.WaveColor.ToCGColor();
        CGColor cgColor2;
        if (this.FormsShimmer.CustomView != null && !this.GetShimmerViewRenderers(Xamarin.Forms.Platform.iOS.Platform.GetRenderer((VisualElement) this.FormsShimmer.CustomView) as UIView).Any<ShimmerViewRenderer>())
        {
          cgColor2 = this.FormsShimmer.WaveColor.MultiplyAlpha(0.08).ToCGColor();
        }
        else
        {
          cgColor2 = this.FormsShimmer.Color.ToCGColor();
          context.SetBlendMode(CGBlendMode.SourceAtop);
        }
        this.gradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor[3]
        {
          cgColor2,
          cgColor1,
          cgColor2
        });
      }
      context.SaveState();
      context.AddRect(this.waveRect);
      context.DrawLinearGradient(this.gradient, startPoint, endPoint, CGGradientDrawingOptions.None);
      context.RestoreState();
    }

    private (double x, double y) GetPosition(VisualElement view)
    {
      double x = view.X;
      double y = view.Y;
      for (VisualElement visualElement = view.Parent as VisualElement; visualElement != null; visualElement = !(visualElement.Parent is LearnerShimmer) ? visualElement.Parent as VisualElement : (VisualElement) null)
      {
        x += visualElement.X;
        y += visualElement.Y;
      }
      return (x, y);
    }

    private IEnumerable<ShimmerViewRenderer> GetShimmerViewRenderers(
      UIView view)
    {
      if (view is ShimmerViewRenderer shimmerViewRenderer1 && !shimmerViewRenderer1.Hidden)
        yield return shimmerViewRenderer1;
      else if (view != null)
      {
        UIView[] uiViewArray = view.Subviews;
        for (int index = 0; index < uiViewArray.Length; ++index)
        {
          foreach (ShimmerViewRenderer shimmerViewRenderer in this.GetShimmerViewRenderers(uiViewArray[index]))
            yield return shimmerViewRenderer;
        }
        uiViewArray = (UIView[]) null;
      }
    }
  }
}
