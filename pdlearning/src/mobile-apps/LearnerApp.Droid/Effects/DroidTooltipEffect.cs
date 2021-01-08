using System;
using Android.Views;
using Com.Tomergoldst.Tooltips;
using LearnerApp.Common.Enum;
using LearnerApp.Droid.Effects;
using LearnerApp.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;

[assembly: ResolutionGroupName("LearnerApp")]
[assembly: ExportEffect(typeof(DroidTooltipEffect), nameof(TooltipEffect))]

namespace LearnerApp.Droid.Effects
{
    public class DroidTooltipEffect : PlatformEffect
    {
        private readonly ToolTipsManager _toolTipsManager;
        private ToolTip toolTipView;

        public DroidTooltipEffect()
        {
            ITipListener listener = new TipListener();
            _toolTipsManager = new ToolTipsManager(listener);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;
            control.Click += OnTap;
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.Click -= OnTap;
            _toolTipsManager.FindAndDismiss(control);
        }

        private void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;

            var text = TooltipEffect.GetText(Element);

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var parentContent = control.RootView;

            var position = TooltipEffect.GetPosition(Element);

            var builder = position switch
            {
                TooltipPosition.Top => new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionAbove),
                TooltipPosition.Left => new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionLeftTo),
                TooltipPosition.Right => new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionRightTo),
                _ => new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionBelow)
            };

            builder.SetAlign(ToolTip.AlignLeft);
            builder.SetBackgroundColor(TooltipEffect.GetBackgroundColor(Element).ToAndroid());
            builder.SetTextColor(TooltipEffect.GetTextColor(Element).ToAndroid());

            toolTipView = builder.Build();

            _toolTipsManager?.Show(toolTipView);
        }
    }
}
