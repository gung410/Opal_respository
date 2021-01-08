using System;
using LearnerApp.Controls.DigitalContentPlayer;
using LearnerApp.Controls.Learner;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class LearningContentView
    {
        private readonly LearningContentViewModel _viewModel;

        public LearningContentView()
        {
            InitializeComponent();

            _viewModel = (LearningContentViewModel)BindingContext;
            var digitalContentPlayerFullscreenHandler = new DigitalContentPlayerFullscreenHandler(
                this,
                new VisualElement[] { HeaderSection, ProgressBarSection });

            MessagingCenter.Subscribe<LearningOpportunityFeedbackCard>(this, "on-close-submit-feedback", (sender) =>
            {
                NavigationService.GoBack();
            });

            digitalContentPlayerFullscreenHandler.Init();
            _viewModel.SetFullscreenPlayerHandler(digitalContentPlayerFullscreenHandler);
        }

        ~LearningContentView()
        {
            MessagingCenter.Unsubscribe<LearningOpportunityFeedbackCard>(this, "on-close-submit-feedback");
        }

        private void Closed_Tapped(object sender, EventArgs e)
        {
            if (_viewModel.IsDownloading)
            {
                return;
            }

            NavigationService.GoBack();
        }
    }
}
