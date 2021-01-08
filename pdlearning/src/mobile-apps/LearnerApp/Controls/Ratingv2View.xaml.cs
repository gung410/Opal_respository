using System;
using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ratingv2View : ContentView
    {
        /// <summary>
        /// Have to set size for visible control.
        /// </summary>
        public static readonly BindableProperty StarSizeProperty = BindableProperty.Create(nameof(StarSize), typeof(int), typeof(Ratingv2View), null, propertyChanged: OnStarSizeChanged);
        public static readonly BindableProperty StarSpacingProperty = BindableProperty.Create(nameof(StarSpacing), typeof(int), typeof(Ratingv2View), null, propertyChanged: OnStarSpacingChanged);
        public static readonly BindableProperty RateProperty = BindableProperty.Create(nameof(Rate), typeof(double), typeof(Ratingv2View), -1.0, propertyChanged: OnRateChanged);
        public static readonly BindableProperty IsEnableRateProperty = BindableProperty.Create(nameof(IsEnableRate), typeof(bool), typeof(Ratingv2View), null);
        public static readonly BindableProperty RateChangedCommandProperty = BindableProperty.Create(nameof(RateChangedCommand), typeof(ICommand), typeof(Ratingv2View), null);

        public Ratingv2View()
        {
            InitializeComponent();
        }

        public event EventHandler<int> RateChanged;

        public ICommand RateChangedCommand
        {
            get { return (ICommand)GetValue(RateChangedCommandProperty); }
            set { SetValue(RateChangedCommandProperty, value); }
        }

        public int StarSize
        {
            get { return (int)GetValue(StarSizeProperty); }
            set { SetValue(StarSizeProperty, value); }
        }

        public int StarSpacing
        {
            get { return (int)GetValue(StarSpacingProperty); }
            set { SetValue(StarSpacingProperty, value); }
        }

        public double Rate
        {
            get { return (double)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }

        public bool IsEnableRate
        {
            get { return (bool)GetValue(IsEnableRateProperty); }
            set { SetValue(IsEnableRateProperty, value); }
        }

        private static void OnStarSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            int size = (int)newValue;
            var view = bindable as Ratingv2View;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.Star1.WidthRequest = size;
                view.Star1.HeightRequest = size;
                view.Star2.WidthRequest = size;
                view.Star2.HeightRequest = size;
                view.Star3.WidthRequest = size;
                view.Star3.HeightRequest = size;

                view.RatingStack.IsVisible = true;
            });
        }

        private static void OnStarSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Ratingv2View;

            Device.BeginInvokeOnMainThread(() =>
            {
                view.RatingStack.Spacing = (int)newValue;
            });
        }

        private static void OnRateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Ratingv2View;

            Device.BeginInvokeOnMainThread(() =>
            {
                var rateValue = (int)Math.Round((double)newValue, 1);
                if (rateValue < 0 || rateValue > 3)
                {
                    return;
                }

                view.RenderStar(rateValue);
            });
        }

        private void Rating_Tapped(object sender, EventArgs e)
        {
            if (!IsEnableRate)
            {
                return;
            }

            SvgCachedImage star = sender as SvgCachedImage;

            int index = RatingStack.Children.IndexOf(star);

            if (index < 0)
            {
                return;
            }

            int rateValue = index + 1;

            RenderStar(rateValue);

            RateChangedCommand?.Execute(rateValue);
            RateChanged?.Invoke(this, rateValue);
        }

        private void RenderStar(int value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                for (int i = 0; i < RatingStack.Children.Count; ++i)
                {
                    ((SvgCachedImage)RatingStack.Children[i]).Source = i < value ? "black_star.svg" : "white_star.svg";
                }
            });
        }
    }
}
