using System;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models.Learner;
using LearnerApp.ViewModels;
using LearnerApp.Views;
using LearnerApp.Views.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.Learner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LearningOpportunityFeedbackCard : ContentView
    {
        public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(FeedbackDataTransfer), typeof(LearningOpportunityFeedbackCard), null, propertyChanged: OnCourseIdChanged);

        private static LearningOpportunityFeedbackCardViewModel _viewModel;

        private static LearningOpportunityFeedbackCard _view;

        private static int _ratingValue;

        public LearningOpportunityFeedbackCard()
        {
            InitializeComponent();

            _viewModel = new LearningOpportunityFeedbackCardViewModel();
        }

        public FeedbackDataTransfer Data
        {
            get { return (FeedbackDataTransfer)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnCourseIdChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(newValue is FeedbackDataTransfer data))
            {
                return;
            }

            _view = (LearningOpportunityFeedbackCard)bindable;

            _ratingValue = 0;

            var isCourseType = data.ItemType == PdActivityType.Courses;

            if (isCourseType && !data.IsMicrolearningType)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _view.RatingStack.IsVisible = false;
                });
            }

            if (data.OwnReview != null)
            {
                _ratingValue = (int)Math.Round((double)data.OwnReview.Rate, 1);
                _view.RatingStack.Rate = _ratingValue;

                Device.BeginInvokeOnMainThread(() =>
                {
                    var loadComment = data.OwnReview.CommentContent;
                    _view.CommentLabel.Text = loadComment;
                    _view.EditCommentEntry.Text = loadComment;

                    if (data.HasContentChanged)
                    {
                        return;
                    }

                    _view.RatingStack.IsEnableRate = false;
                    _view.SubmitStack.IsEnabled = isCourseType;
                    _view.SubmitStack.BackgroundColor = isCourseType
                        ? (Color)Application.Current.Resources["MainBodyTextColor"]
                        : (Color)Application.Current.Resources["RegisteredColor"];
                    _view.EditCommentButton.IsVisible = isCourseType;
                    _view.EditCommentStack.IsVisible = false;

                    if (loadComment.IsNullOrEmpty())
                    {
                        _view.EditCommentStack.IsVisible = true;
                        _view.CommentStack.IsVisible = false;
                    }
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _view.RatingStack.IsEnableRate = true;
                    _view.EditCommentStack.IsVisible = true;
                    _view.CommentStack.IsVisible = false;
                });
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                _view.CourseNameLbl.Text = $"You have completed \"{data.ContentTitle}\".";
            });
        }

        private async void SubmitRating_Tapped(object sender, EventArgs e)
        {
            switch (Data.ItemType)
            {
                case PdActivityType.Courses:
                    var courseReview = new
                    {
                        ItemId = Data.ContentId,
                        Rating = _ratingValue,
                        CommentContent = _view.EditCommentEntry.Text,
                        ItemType = "Course"
                    };

                    if (Data.IsMicrolearningType)
                    {
                        if (!string.IsNullOrEmpty(Data.ContentId) && _ratingValue != 0)
                        {
                            _view.RatingErrorMessage.IsVisible = false;

                            // Submit and resubmit comment base on HasContentChanged
                            if (Data.OwnReview == null)
                            {
                                await SubmitFeedback(courseReview, PdActivityType.Courses);
                            }
                            else
                            {
                                await ReSubmitFeedback(Data.ContentId, courseReview);
                            }
                        }
                        else
                        {
                            _view.RatingErrorMessage.IsVisible = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Data.ContentId) && !string.IsNullOrEmpty(EditCommentEntry.Text))
                        {
                            _view.EditCommentStack.BorderColor = Color.LightGray;
                            _view.CommentErrorMessage.IsVisible = false;

                            // Submit and resubmit comment base on Data.OwnReview
                            if (Data.OwnReview == null)
                            {
                                await SubmitFeedback(courseReview, PdActivityType.Courses);
                            }
                            else
                            {
                                await ReSubmitFeedback(Data.ContentId, courseReview);
                            }
                        }
                        else
                        {
                            _view.EditCommentStack.BorderColor = Color.Red;
                            _view.CommentErrorMessage.IsVisible = true;
                        }
                    }

                    break;
                case PdActivityType.DigitalContent:
                    var digitalContentReview = new
                    {
                        ItemId = Data.ContentId,
                        Rating = _ratingValue,
                        CommentContent = _view.EditCommentEntry.Text,
                        ItemType = PdActivityType.DigitalContent.ToString()
                    };

                    await SubmitFeedback(digitalContentReview, PdActivityType.DigitalContent);
                    break;
            }
        }

        private async Task SubmitFeedback(object reviewItem, PdActivityType myLearningType)
        {
            bool submitted = await _viewModel.SubmitFeedback(reviewItem);

            switch (myLearningType)
            {
                case PdActivityType.Courses:
                    SubmitCourseEvent(submitted);
                    break;
                case PdActivityType.DigitalContent:
                    SubmitDigitalContentEvent(submitted);
                    break;
            }
        }

        private async Task ReSubmitFeedback(string itemId, object reviewItem)
        {
            bool submitted = await _viewModel.ReSubmitFeedback(itemId, reviewItem);
            SubmitCourseEvent(submitted);
        }

        private void SubmitCourseEvent(bool submitted)
        {
            if (!submitted)
            {
                return;
            }

            MessagingCenter.Unsubscribe<CourseDetailsViewModel>(this, "on-submit-feedback");
            MessagingCenter.Send(this, "on-submit-feedback");

            MessagingCenter.Unsubscribe<LearningContentView>(this, "on-close-submit-feedback");
            MessagingCenter.Send(this, "on-close-submit-feedback");
        }

        private void SubmitDigitalContentEvent(bool submitted)
        {
            if (!submitted)
            {
                return;
            }

            MessagingCenter.Unsubscribe<MyDigitalContentLearningView>(this, "on-close-submit-digital-feedback");
            MessagingCenter.Send(this, "on-close-submit-digital-feedback");
        }

        private void EditComment_Tapped(object sender, System.EventArgs e)
        {
            _view.RatingStack.IsEnableRate = true;
            _view.CommentStack.IsVisible = false;
            _view.EditCommentStack.IsVisible = true;
            _view.EditCommentEntry.IsEnabled = true;
            _view.SubmitStack.IsEnabled = true;
        }

        private void RatingStack_RateChanged(object sender, int e)
        {
            _ratingValue = e;
        }
    }
}
