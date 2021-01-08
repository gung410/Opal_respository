using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

#pragma warning disable
namespace LearnerApp.iOS.Helpers.Spring
{
    public class SpringViewWrapper : ISpringable
    {
        [Weak] UIView _view;
        private readonly Spring _spring;

        public SpringViewWrapper(UIView view)
        {
            _view = view;
            _spring = new Spring(this);
        }

        public bool Autostart { get; set; } = false;
        public bool Autohide { get; set; } = false;

        public Spring.AnimationPreset? Animation { get; set; }
        public float Force { get; set; } = 1;
        public float Delay { get; set; } = 0;
        public float Duration { get; set; } = 0.7f;
        public float Damping { get; set; } = 0.7f;
        public float Velocity { get; set; }= 0.7f;
        public float RepeatCount { get; set; } = 1;
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        public double Rotate { get; set; } = 0;
        public float Opacity { get; set; } = 1;
        public bool AnimateFrom { get; set; } = false;
        public Spring.AnimationCurve Curve { get; set; }
        public CALayer Layer => _view.Layer;

        public CGAffineTransform Transform
        {
            get => _view.Transform;
            set => _view.Transform = value;
        }

        public float Alpha
        {
            get => (float)_view.Alpha;
            set => _view.Alpha = value;
        }

        public void Animate()
        {
            _spring.Animate();
        }

        public void AnimateNext(Action completion)
        {
            _spring.AnimateNext(completion);
        }

        public void AnimateTo()
        {
            _spring.AnimateTo();
        }

        public void AnimateToNext(Action completion)
        {
            _spring.AnimateToNext(completion);
        }

        public void UpdateViewIfNeeded(UIView contentView)
        {
            if (!contentView.Equals(_view))
            {
                _view = contentView;
            }
        }
    }
}
