using System;
using System.Linq;
using System.Windows.Input;
using LearnerApp.Common;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.Learner
{
    public partial class LearningOpportunityReviewsCard : ContentView
    {
        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(
                nameof(Items),
                typeof(EditReview),
                typeof(LearningOpportunityReviewsCard),
                null,
                propertyChanged: OnItemsChanged);

        public static readonly BindableProperty EditCommentCommandProperty = BindableProperty.Create(nameof(EditCommentCommand), typeof(ICommand), typeof(Ratingv2View), null);

        public LearningOpportunityReviewsCard()
        {
            InitializeComponent();
        }

        public ICommand EditCommentCommand
        {
            get { return (ICommand)GetValue(EditCommentCommandProperty); }
            set { SetValue(EditCommentCommandProperty, value); }
        }

        public EditReview Items
        {
            get { return (EditReview)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is EditReview editReview))
            {
                return;
            }

            if (editReview.UserReviews.IsNullOrEmpty())
            {
                return;
            }

            var view = (LearningOpportunityReviewsCard)bindable;

            Device.BeginInvokeOnMainThread(() =>
            {
                var reviews = editReview.UserReviews;
                view.ReviewStack.IsVisible = true;

                string vote = reviews.Count > 1 ? "reviews" : "review";
                string review = reviews.Count > 1 ? "REVIEWS" : "REVIEW";
                view.ReviewCountLbl.Text = $"{review} ({reviews.Count})";
                view.TotalReviewStack.IsVisible = !reviews.IsNullOrEmpty();
                view.TotalRatingStack.IsVisible = editReview.IsMicrolearningType;
                view.TotalReview.Text = $"{reviews.Count} {vote}";

                double averageRating = reviews.Sum(p => p.Rate) / reviews.Count;
                view.TotalRatingValue.Rate = averageRating;
                view.TotalRating.Text = $"({Math.Round(averageRating, 1)})";

                foreach (var reviewDetail in reviews)
                {
                    reviewDetail.IsVisibleRating = editReview.IsMicrolearningType;

                    if (reviewDetail.ChangedDate == null)
                    {
                        continue;
                    }

                    reviewDetail.CreatedDate = reviewDetail.ChangedDate.Value;
                }

                view.ListReviews.ItemsSource = reviews;
            });
        }

        private void EditComment_Tapped(object sender, EventArgs e)
        {
            EditCommentCommand?.Execute((e as TappedEventArgs)?.Parameter as UserReview);
        }
    }
}
