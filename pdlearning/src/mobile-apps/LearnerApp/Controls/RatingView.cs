using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public class RatingView : ContentView
    {
        public static BindableProperty RatingNumberProperty =
            BindableProperty.Create(
                nameof(RatingNumber),
                typeof(int),
                typeof(RatingView),
                -1, // OnRatingNumberChanged not receive 0 when load page. [IMPORTANT] PLEASE LET IT -1. DO NOT FREAKING CHANGE THIS VALUE!
                propertyChanged: OnRatingNumberChanged);

        public int RatingNumber
        {
            get { return (int)GetValue(RatingNumberProperty); }
            set { SetValue(RatingNumberProperty, value); }
        }

        private static void OnRatingNumberChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ratingNumber = (int)newValue;
            if (ratingNumber < 0 || ratingNumber > 3)
            {
                return;
            }

            var stackLayout = new StackLayout
            {
                Spacing = 2,
                Orientation = StackOrientation.Horizontal
            };

            // The star number can be 0 1 2 3.
            // The maximum star to be rendered is 3.
            for (int star = 0; star < 3; star++)
            {
                stackLayout.Children.Add(star < ratingNumber ? RatingStar.Rated.Image : RatingStar.NotRated.Image);
            }

            ((RatingView)bindable).Content = stackLayout;
        }

        private class RatingStar
        {
            public RatingStar(bool isRated)
            {
                IsRated = isRated;
            }

            public static RatingStar Rated
            {
                get
                {
                    return new RatingStar(true);
                }
            }

            public static RatingStar NotRated
            {
                get
                {
                    return new RatingStar(false);
                }
            }

            /// <summary>
            /// When IsRated = true, the icon should be the black star.
            /// </summary>
            public bool IsRated { get; }

            public string IconString => IsRated ? "black_star.svg" : "white_star.svg";

            public SvgCachedImage Image => new SvgCachedImage
            {
                Source = IconString,
                HeightRequest = (OnPlatform<double>)Application.Current.Resources["LittleSize"],
                WidthRequest = (OnPlatform<double>)Application.Current.Resources["LittleSize"]
            };
        }
    }
}
