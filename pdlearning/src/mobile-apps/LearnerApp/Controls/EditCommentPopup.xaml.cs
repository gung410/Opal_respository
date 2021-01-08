using System;
using LearnerApp.Models.Learner;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCommentPopup
    {
        public EventHandler<UserReview> OnSaveEditCommentEventHandler;

        private UserReview _review;
        private bool _isMicroLearningType;
        private double _newRateValue;

        public EditCommentPopup(UserReview review, bool isMicroLearningType)
        {
            InitializeComponent();

            _review = review;
            _isMicroLearningType = isMicroLearningType;

            Device.BeginInvokeOnMainThread(() =>
            {
                RatingStackLayout.IsVisible = isMicroLearningType;
                RatingView.Rate = _review.Rate;
                CommentEditor.Text = _review.CommentContent;
            });
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            if (_isMicroLearningType && (_newRateValue == 0 || string.IsNullOrEmpty(CommentEditor.Text)))
            {
                return;
            }

            if (!_isMicroLearningType && string.IsNullOrEmpty(CommentEditor.Text))
            {
                return;
            }

            _review.Rate = _newRateValue;
            _review.CommentContent = CommentEditor.Text;

            OnSaveEditCommentEventHandler?.Invoke(this, _review);
            PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void Closed_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void RatingView_RateChanged(object sender, int e)
        {
            _newRateValue = e;
        }
    }
}
