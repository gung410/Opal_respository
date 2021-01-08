using System;
using System.ComponentModel;
using LearnerApp.Common.Enum;
using LearnerApp.Effects;
using LearnerApp.iOS.Effects;
using LearnerApp.iOS.Helpers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(IOSTooltipEffect), nameof(TooltipEffect))]

namespace LearnerApp.iOS.Effects
{
    public class IOSTooltipEffect : PlatformEffect
    {
        private readonly EasyTipView.EasyTipView _tooltip;
        private UITapGestureRecognizer _tapGestureRecognizer;

        public IOSTooltipEffect()
        {
            _tooltip = new EasyTipView.EasyTipView();
            _tooltip.DidDismiss += OnDismiss;
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            if (control is UIButton)
            {
                if (Control is UIButton btn)
                {
                    btn.TouchUpInside += OnTap;
                }
            }
            else
            {
                _tapGestureRecognizer = new UITapGestureRecognizer(obj =>
                {
                    OnTap(obj, EventArgs.Empty);
                });
                control.UserInteractionEnabled = true;
                control.AddGestureRecognizer(_tapGestureRecognizer);
            }
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;

            if (control is UIButton)
            {
                if (Control is UIButton btn)
                {
                    btn.TouchUpInside -= OnTap;
                }
            }
            else
            {
                if (_tapGestureRecognizer != null)
                {
                    control.RemoveGestureRecognizer(_tapGestureRecognizer);
                }
            }

            _tooltip?.Dismiss();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == TooltipEffect.BackgroundColorProperty.PropertyName)
            {
                _tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
            }
            else if (args.PropertyName == TooltipEffect.TextColorProperty.PropertyName)
            {
                _tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
            }
            else if (args.PropertyName == TooltipEffect.TextProperty.PropertyName)
            {
                _tooltip.Text = new Foundation.NSString(TooltipEffect.GetText(Element));
            }
            else if (args.PropertyName == TooltipEffect.PositionProperty.PropertyName)
            {
                UpdatePosition();
            }
        }

        private void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;

            var text = TooltipEffect.GetText(Element);

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _tooltip.BubbleColor = TooltipEffect.GetBackgroundColor(Element).ToUIColor();
            _tooltip.ForegroundColor = TooltipEffect.GetTextColor(Element).ToUIColor();
            _tooltip.Text = new Foundation.NSString(text);
            UpdatePosition();

            UIView superView = FindScrollView(control) ?? control.FindViewController().View;

            _tooltip?.Show(control, superView, true);
        }

        private UIScrollView FindScrollView(UIView view)
        {
            UIView superview = view;
            while (true)
            {
                superview = superview.Superview;
                if (superview == null)
                {
                    break;
                }

                if (superview.Superview is UIScrollView scrollView)
                {
                    return scrollView;
                }
            }

            return null;
        }

        private void OnDismiss(object sender, EventArgs e)
        {
            // do something on dismiss
        }

        private void UpdatePosition()
        {
            var position = TooltipEffect.GetPosition(Element);

            _tooltip.ArrowPosition = position switch
            {
                TooltipPosition.Top => EasyTipView.ArrowPosition.Bottom,
                TooltipPosition.Left => EasyTipView.ArrowPosition.Right,
                TooltipPosition.Right => EasyTipView.ArrowPosition.Left,
                _ => EasyTipView.ArrowPosition.Top
            };
        }
    }
}
