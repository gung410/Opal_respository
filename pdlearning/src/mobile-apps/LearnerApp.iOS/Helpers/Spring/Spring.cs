using System;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

#pragma warning disable
namespace LearnerApp.iOS.Helpers.Spring
{
    /// <summary>
    /// PORT FROM: https://github.com/MengTo/Spring
    /// </summary>
    public interface ISpringable {
        bool Autostart { get; set; }
        bool Autohide { get; set; }
        Spring.AnimationPreset? Animation { get; set; }
        float Force { get; set; }
        float Delay { get; set; }
        float Duration { get; set; }
        float Damping { get; set; }
        float Velocity { get; set; }
        float RepeatCount { get; set; }
        float X  { get; set; }
        float Y  { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        double Rotate { get; set; }
        float Opacity  { get; set; }
        bool AnimateFrom  { get; set; }
        Spring.AnimationCurve Curve  { get; set; }

        CALayer Layer { get; }
        CGAffineTransform Transform { get; set; }
        float Alpha { get; set; }

        void Animate();
        void AnimateNext(Action completion);
        void AnimateTo();
        void AnimateToNext(Action completion);
    }


    public class Spring : NSObject
    {
        [Weak] ISpringable _view;
        private bool _shouldAnimateAfterActive = false;
        private bool _shouldAnimateInLayoutSubviews = true;
        private readonly NSObject _didBecomeActiveObserver;

        public Spring(ISpringable view) : base()
        {
            this._view = view;
            _didBecomeActiveObserver =
                NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification,
                    DidBecomeActive);
        }

        ~Spring()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveObserver);
        }

        private void DidBecomeActive(NSNotification obj)
        {
            if (_shouldAnimateAfterActive)
            {
                Alpha = 0;
                Animate();
                _shouldAnimateAfterActive = false;
            }
        }

        private bool Autostart
        {
            get => _view.Autostart;
            set => _view.Autostart = value;
        }

        private bool Autohide
        {
            get => _view.Autohide;
            set => _view.Autohide = value;
        }

        private AnimationPreset? Animation
        {
            get => _view.Animation;
            set => _view.Animation = value;
        }

        private float Force
        {
            get => _view.Force;
            set => _view.Force = value;
        }

        private float Delay
        {
            get => _view.Delay;
            set => _view.Delay = value;
        }

        private float Duration
        {
            get => _view.Duration;
            set => _view.Duration = value;
        }

        private float Damping
        {
            get => _view.Damping;
            set => _view.Damping = value;
        }

        private float Velocity
        {
            get => _view.Velocity;
            set => _view.Velocity = value;
        }

        private float RepeatCount
        {
            get => _view.RepeatCount;
            set => _view.RepeatCount = value;
        }

        private float X
        {
            get => _view.X;
            set => _view.X = value;
        }

        private float Y
        {
            get => _view.Y;
            set => _view.Y = value;
        }

        private float ScaleX
        {
            get => _view.ScaleX;
            set => _view.ScaleX = value;
        }

        private float ScaleY
        {
            get => _view.ScaleY;
            set => _view.ScaleY = value;
        }

        private double Rotate
        {
            get => _view.Rotate;
            set => _view.Rotate = value;
        }

        private float Opacity
        {
            get => _view.Velocity;
            set => _view.Velocity = value;
        }

        private bool AnimateFrom
        {
            get => _view.AnimateFrom;
            set => _view.AnimateFrom = value;
        }

        private AnimationCurve Curve
        {
            get => _view.Curve;
            set => _view.Curve = value;
        }

        // UI VIew
        private CALayer Layer
        {
            get => _view.Layer;
        }

        private CGAffineTransform Transform
        {
            get => _view.Transform;
            set => _view.Transform = value;
        }

        private float Alpha
        {
            get => _view.Alpha;
            set => _view.Alpha = value;
        }

        public enum AnimationPreset
        {
            SlideLeft,
            SlideRight,
            SlideDown,
            SlideUp,
            SqueezeLeft,
            SqueezeRight,
            SqueezeDown,
            SqueezeUp,
            FadeIn,
            FadeOut,
            FadeOutIn,
            FadeInLeft,
            FadeInRight,
            FadeInDown,
            FadeInUp,
            ZoomIn,
            ZoomOut,
            Fall,
            Shake,
            Pop,
            FlipX,
            FlipY,
            Morph,
            Squeeze,
            Flash,
            Wobble,
            Swing,
        }

        public enum AnimationCurve
        {
            EaseIn,
            EaseOut,
            EaseInOut,
            Linear,
            Spring,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
        }

        public void AnimatePreset()
        {
            Alpha = 0.99f;
            switch (Animation) {
            case AnimationPreset.SlideLeft:
                this.X = 300 * Force;
                break;
            case AnimationPreset.SlideRight:
                this.X = -300 * Force;
                break;
            case AnimationPreset.SlideDown:
                this.Y = -300 * Force;
                break;
            case AnimationPreset.SlideUp:
                this.Y = 300 * Force;
                break;
            case AnimationPreset.SqueezeLeft:
                this.X = 300;
                ScaleX = 3 * Force;
                break;
            case AnimationPreset.SqueezeRight:
                this.X = -300;
                ScaleX = 3 * Force;
                break;
            case AnimationPreset.SqueezeDown:
                Y = -300;
                ScaleY = 3*Force;
                break;
            case AnimationPreset.SqueezeUp:
                Y = 300;
                ScaleY = 3 * Force;
                break;
            case AnimationPreset.FadeIn:
                Opacity = 0;
                break;
            case AnimationPreset.FadeOut:
                AnimateFrom = false;
                Opacity = 0;
                break;
            case AnimationPreset.FadeOutIn:
                var fadeOutInAnimation = new CABasicAnimation();
                fadeOutInAnimation.KeyPath = "opacity";
                fadeOutInAnimation.From = new NSNumber(1.0f);
                fadeOutInAnimation.To =  new NSNumber(0.0f);;
                fadeOutInAnimation.TimingFunction = GetTimingFunction(curve: Curve);
                fadeOutInAnimation.Duration = Duration;
                fadeOutInAnimation.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                fadeOutInAnimation.AutoReverses = true;
                Layer.AddAnimation(fadeOutInAnimation, "fade");
                break;
            case AnimationPreset.FadeInLeft:
                Opacity = 0;
                this.X = 300 * Force;
                break;
            case AnimationPreset.FadeInRight:
                this.X = -300 * Force;
                Opacity = 0;
                break;
            case AnimationPreset.FadeInDown:
                Y = -300*Force;
                Opacity = 0;
                break;
            case AnimationPreset.FadeInUp:
                Y = 300 * Force;
                Opacity = 0;
                break;
            case AnimationPreset.ZoomIn:
                Opacity = 0;
                ScaleX = 2 * Force;
                ScaleY = 2 * Force;
                break;

            case AnimationPreset.ZoomOut:
                AnimateFrom = false;
                Opacity = 0;
                ScaleX = 2 * Force;
                ScaleY = 2 * Force;
                break;
            case AnimationPreset.Fall:
                AnimateFrom = false;
                Rotate = 15 * Math.PI / 180.0f;
                Y = 600 * Force;
                break;

            case AnimationPreset.Shake:
                var keyFrameAnimation = new CAKeyFrameAnimation();
                keyFrameAnimation.KeyPath = "position.x";
                keyFrameAnimation.Values = new[] {0f, 30 * Force, -30 * Force, 30 * Force, 0}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();

                keyFrameAnimation.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                keyFrameAnimation.TimingFunction = GetTimingFunction(curve: Curve);
                keyFrameAnimation.Duration = Duration;
                keyFrameAnimation.Additive = true;
                keyFrameAnimation.RepeatCount = RepeatCount;
                keyFrameAnimation.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(keyFrameAnimation, "shake");
                break;

            case AnimationPreset.Pop:
                var popAnimation = new CAKeyFrameAnimation();
                popAnimation.KeyPath = "transform.scale";
                popAnimation.Values = new[] {0, 0.2*Force, -0.2*Force, 0.2*Force, 0}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                popAnimation.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                popAnimation.TimingFunction = GetTimingFunction(curve: Curve);
                popAnimation.Duration = Duration;
                popAnimation.Additive = true;
                popAnimation.RepeatCount = RepeatCount;
                popAnimation.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(popAnimation, "shake");
                break;

            case AnimationPreset.FlipX:
                Rotate = 0;
                ScaleX = 1;
                ScaleY = 1;
                var flipXPerspective = CATransform3D.Identity;
                flipXPerspective.m34 = (float) -1.0 / Layer.Frame.Size.Width / 2;

                var flipXAnimation = new CABasicAnimation();
                flipXAnimation.KeyPath = "transform";
                flipXAnimation.From = NSValue.FromCATransform3D(CATransform3D.MakeRotation(0, 0, 0, 0));
                flipXAnimation.To = NSValue.FromCATransform3D(
                    flipXPerspective.Concat(CATransform3D.MakeRotation((float) Math.PI, 0, 1, 0)));

                flipXAnimation.TimingFunction = GetTimingFunction(curve: Curve);
                flipXAnimation.Duration = Duration;
                flipXAnimation.Additive = true;
                flipXAnimation.RepeatCount = RepeatCount;
                Layer.AddAnimation(flipXAnimation, "3d");
                break;

            case AnimationPreset.FlipY:
                Rotate = 0;
                ScaleX = 1;
                ScaleY = 1;
                var flipYPerspective = CATransform3D.Identity;
                flipYPerspective.m34 = (float) -1.0 / Layer.Frame.Size.Width / 2;

                var flipYAnimation = new CABasicAnimation();
                flipYAnimation.KeyPath = "transform";
                flipYAnimation.From = NSValue.FromCATransform3D(CATransform3D.MakeRotation(0, 0, 0, 0));
                flipYAnimation.To = NSValue.FromCATransform3D(
                    flipYPerspective.Concat(CATransform3D.MakeRotation((float) Math.PI, 1, 0, 0)));

                flipYAnimation.TimingFunction = GetTimingFunction(Curve);
                flipYAnimation.Duration = Duration;
                flipYAnimation.Additive = true;
                flipYAnimation.RepeatCount = RepeatCount;
                Layer.AddAnimation(flipYAnimation, "3d");
                break;

            case AnimationPreset.Morph:
                var morphX = new CAKeyFrameAnimation();
                morphX.KeyPath = "transform.scale.x";
                morphX.Values = new[] {1, 1.3*Force, 0.7, 1.3*Force, 1}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                morphX.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                morphX.TimingFunction = GetTimingFunction(Curve);
                morphX.Duration = Duration;
                morphX.RepeatCount = RepeatCount;
                morphX.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(morphX, "morphX");

                var morphY = new CAKeyFrameAnimation();
                morphY.KeyPath = "transform.scale.y";
                morphY.Values = new[] {1, 0.7, 1.3*Force, 0.7, 1}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                morphY.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                morphY.TimingFunction = GetTimingFunction(Curve);
                morphY.Duration = Duration;
                morphY.RepeatCount = RepeatCount;
                morphY.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(morphY, "morphY");
                break;

            case AnimationPreset.Squeeze:
                var squeezeMorphX = new CAKeyFrameAnimation();
                squeezeMorphX.KeyPath = "transform.scale.x";
                squeezeMorphX.Values = new[] {1, 1.5*Force, 0.5, 1.5*Force, 1}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                squeezeMorphX.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                squeezeMorphX.TimingFunction = GetTimingFunction(Curve);
                squeezeMorphX.Duration = Duration;
                squeezeMorphX.RepeatCount = RepeatCount;
                squeezeMorphX.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(squeezeMorphX, "morphX");

                var squeezeMorphY = new CAKeyFrameAnimation();
                squeezeMorphY.KeyPath = "transform.scale.y";
                squeezeMorphY.Values = new[] {1, 0.5, 1, 0.5, 1}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                squeezeMorphY.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                squeezeMorphY.TimingFunction = GetTimingFunction(Curve);
                squeezeMorphY.Duration = Duration;
                squeezeMorphY.RepeatCount = RepeatCount;
                squeezeMorphY.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(squeezeMorphY, "morphY");
                break;
            case AnimationPreset.Flash:
                var flashAnimation = new CABasicAnimation();
                flashAnimation.KeyPath = "opacity";
                flashAnimation.From = new NSNumber(1);
                flashAnimation.To = new NSNumber(0);
                flashAnimation.Duration = Duration;
                flashAnimation.RepeatCount = RepeatCount;
                flashAnimation.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(flashAnimation, "flash");
                break;

            case AnimationPreset.Wobble:
                var wobbleMorphX = new CAKeyFrameAnimation();
                wobbleMorphX.KeyPath = "transform.rotation";
                wobbleMorphX.Values = new[] {0, 0.3*Force, -0.3*Force, 0.3*Force, 0}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                wobbleMorphX.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                wobbleMorphX.TimingFunction = GetTimingFunction(Curve);
                wobbleMorphX.Duration = Duration;
                wobbleMorphX.Additive = true;
                wobbleMorphX.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(wobbleMorphX, "wobble");

                var woobleMorphY = new CAKeyFrameAnimation();
                woobleMorphY.KeyPath = "position.x";
                woobleMorphY.Values = new[] {0, 30*Force, -30*Force, 30*Force, 0}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                woobleMorphY.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};
                woobleMorphY.TimingFunction = GetTimingFunction(Curve);
                woobleMorphY.Duration = Duration;
                wobbleMorphX.Additive = true;
                woobleMorphY.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                Layer.AddAnimation(woobleMorphY, "x");
                break;

            case AnimationPreset.Swing:
                var swingAnimation = new CAKeyFrameAnimation();
                swingAnimation.KeyPath = "transform.rotation";

                swingAnimation.Values = new[] {0, 0.3*Force, -0.3*Force, 0.3*Force, 0}
                    .Select(nb => new NSNumber(nb))
                    .ToArray();
                swingAnimation.KeyTimes = new NSNumber[] {0, 0.2, 0.4, 0.6, 0.8, 1};

                swingAnimation.Duration = Duration;
                swingAnimation.Additive = true;
                swingAnimation.BeginTime = CAAnimation.CurrentMediaTime() +  Delay;
                swingAnimation.RepeatCount = RepeatCount;
                Layer.AddAnimation(swingAnimation, "swing");
                break;
            }
        }

        CAMediaTimingFunction GetTimingFunction(AnimationCurve curve) {
            switch (curve) {
                case AnimationCurve.EaseIn: return CAMediaTimingFunction.FromName(name: CAMediaTimingFunction.EaseIn);
                case AnimationCurve.EaseOut: return CAMediaTimingFunction.FromName(name: CAMediaTimingFunction.EaseOut);
                case AnimationCurve.EaseInOut: return CAMediaTimingFunction.FromName(name: CAMediaTimingFunction.EaseInEaseOut);
                case AnimationCurve.Linear: return CAMediaTimingFunction.FromName(name: CAMediaTimingFunction.Linear);
                case AnimationCurve.Spring: return CAMediaTimingFunction.FromControlPoints(0.5f, (float)1.1 + (float)(Force/3), 1, 1);
                case AnimationCurve.EaseInSine: return CAMediaTimingFunction.FromControlPoints(0.47f, 0, 0.745f, 0.715f);
                case AnimationCurve.EaseOutSine: return CAMediaTimingFunction.FromControlPoints( 0.39f, 0.575f, 0.565f, 1);
                case AnimationCurve.EaseInOutSine: return CAMediaTimingFunction.FromControlPoints( 0.445f, 0.05f, 0.55f, 0.95f);
                case AnimationCurve.EaseInQuad: return CAMediaTimingFunction.FromControlPoints( 0.55f, 0.085f, 0.68f, 0.53f);
                case AnimationCurve.EaseOutQuad: return CAMediaTimingFunction.FromControlPoints( 0.25f, 0.46f, 0.45f, 0.94f);
                case AnimationCurve.EaseInOutQuad: return CAMediaTimingFunction.FromControlPoints( 0.455f, 0.03f, 0.515f, 0.955f);
                case AnimationCurve.EaseInCubic: return CAMediaTimingFunction.FromControlPoints( 0.55f, 0.055f, 0.675f, 0.19f);
                case AnimationCurve.EaseOutCubic: return CAMediaTimingFunction.FromControlPoints( 0.215f, 0.61f, 0.355f,1);
                case AnimationCurve.EaseInOutCubic: return CAMediaTimingFunction.FromControlPoints( 0.645f, 0.045f, 0.355f, 1);
                case AnimationCurve.EaseInQuart: return CAMediaTimingFunction.FromControlPoints( 0.895f, 0.03f, 0.685f, 0.22f);
                case AnimationCurve.EaseOutQuart: return CAMediaTimingFunction.FromControlPoints( 0.165f, 0.84f, 0.44f, 1);
                case AnimationCurve.EaseInOutQuart: return CAMediaTimingFunction.FromControlPoints( 0.77f, 0, 0.175f, 1);
                case AnimationCurve.EaseInQuint: return CAMediaTimingFunction.FromControlPoints( 0.755f, 0.05f, 0.855f, 0.06f);
                case AnimationCurve.EaseOutQuint: return CAMediaTimingFunction.FromControlPoints( 0.23f, 1, 0.32f, 1);
                case AnimationCurve.EaseInOutQuint: return CAMediaTimingFunction.FromControlPoints( 0.86f, 0, 0.07f, 1);
                case AnimationCurve.EaseInExpo: return CAMediaTimingFunction.FromControlPoints( 0.95f, 0.05f, 0.795f, 0.035f);
                case AnimationCurve.EaseOutExpo: return CAMediaTimingFunction.FromControlPoints( 0.19f, 1, 0.22f, 1);
                case AnimationCurve.EaseInOutExpo: return CAMediaTimingFunction.FromControlPoints( 1, 0, 0, 1);
                case AnimationCurve.EaseInCirc: return CAMediaTimingFunction.FromControlPoints( 0.6f, 0.04f, 0.98f, 0.335f);
                case AnimationCurve.EaseOutCirc: return CAMediaTimingFunction.FromControlPoints( 0.075f, 0.82f, 0.165f, 1);
                case AnimationCurve.EaseInOutCirc: return CAMediaTimingFunction.FromControlPoints( 0.785f, 0.135f, 0.15f, 0.86f);
                case AnimationCurve.EaseInBack: return CAMediaTimingFunction.FromControlPoints( 0.6f, -0.28f, 0.735f, 0.045f);
                case AnimationCurve.EaseOutBack: return CAMediaTimingFunction.FromControlPoints( 0.175f, 0.885f, 0.32f, 1.275f);
                case AnimationCurve.EaseInOutBack: return CAMediaTimingFunction.FromControlPoints( 0.68f, -0.55f, 0.265f, 1.55f);
            }

            return CAMediaTimingFunction.FromName(CAMediaTimingFunction.Default);
        }

        public UIViewAnimationOptions GetAnimationOptions(AnimationCurve curve) {

            switch (curve) {
                case AnimationCurve.EaseIn: return UIViewAnimationOptions.CurveEaseIn;
                case AnimationCurve.EaseOut: return UIViewAnimationOptions.CurveEaseOut;
                case AnimationCurve.EaseInOut: return UIViewAnimationOptions.CurveEaseInOut;
                default: break;
            }

            return UIViewAnimationOptions.CurveLinear;
        }

        public void Animate()
        {
            AnimateFrom = true;
            AnimatePreset();
            SetView(null);
        }

        public void AnimateNext(Action completion)
        {
            AnimateFrom = true;
            AnimatePreset();
            SetView(completion);
        }

        public void AnimateTo()
        {
            AnimateFrom = false;
            AnimatePreset();
            SetView(null);
        }

        public void AnimateToNext(Action completion)
        {
            AnimateFrom = false;
            AnimatePreset();
            SetView(completion);
        }

        public void CustomAwakeFromNib() {
            if (Autohide)
            {
                Alpha = 0;
            }
        }

        public void CustomLayoutSubviews() {
            if (_shouldAnimateInLayoutSubviews)
            {
                _shouldAnimateInLayoutSubviews = false;
                if (Autostart) {
                    if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active)
                    {
                        _shouldAnimateAfterActive = true;
                        return;
                    }

                    Alpha = 0;
                    Animate();
                }
            }
        }

        void SetView(Action completion) {
            if (AnimateFrom)
            {
                var translate = CGAffineTransform.MakeTranslation(this.X,  this.Y);

                var scale = CGAffineTransform.MakeScale(this.ScaleX, this.ScaleY);
                var rotate = CGAffineTransform.MakeRotation((nfloat) this.Rotate);

                var translateAndScale = translate * scale;
                this.Transform = rotate * translateAndScale;
                this.Alpha = this.Opacity;
            }

            UIView.AnimateNotify(
                duration: Duration,
                delay: Delay,
                Damping,
                Velocity,
                GetAnimationOptions(Curve) | UIViewAnimationOptions.AllowUserInteraction,
                () =>
                {
                    if (this.AnimateFrom)
                    {
                        this.Transform = CGAffineTransform.MakeIdentity();
                        this.Alpha = 1;
                    }
                    else
                    {
                        var translate = CGAffineTransform.MakeTranslation(this.X, this.Y);
                        var scale = CGAffineTransform.MakeScale(this.ScaleX, this.ScaleY);
                        var rotate = CGAffineTransform.MakeRotation((nfloat)this.Rotate);
                        var translateAndScale = translate * scale;
                        this.Transform = rotate * translateAndScale;

                        this.Alpha = this.Opacity;
                    }
                }, finished =>
                {
                    completion?.Invoke();
                    this?.ResetAll();
                });
            }

        void Reset()
        {
            X = 0;
            Y = 0;
            Opacity = 1;
        }

        void ResetAll() {
            X = 0;
            Y = 0;
            Animation = null;
            Opacity = 1;
            ScaleX = 1;
            ScaleY = 1;
            Rotate = 0;
            Damping = 0.7f;
            Velocity = 0.7f;
            RepeatCount = 1;
            Delay = 0;
            Duration = 0.7f;
        }
    }
}
