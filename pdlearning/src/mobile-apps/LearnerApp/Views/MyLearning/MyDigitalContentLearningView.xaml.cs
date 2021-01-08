using System;
using LearnerApp.Controls.DigitalContentPlayer;
using LearnerApp.Controls.Learner;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDigitalContentLearningView
    {
        private readonly MyDigitalContentLearningViewModel _viewModel;

        public MyDigitalContentLearningView()
        {
            InitializeComponent();

            _viewModel = BindingContext as MyDigitalContentLearningViewModel;
            var digitalContentPlayerFullscreenHandler = new DigitalContentPlayerFullscreenHandler(
                this,
                new VisualElement[] { HeaderSection, ProgressSection, CompleteButton });
            _viewModel.SetFullscreenPlayerHandler(digitalContentPlayerFullscreenHandler);

            MessagingCenter.Subscribe<LearningOpportunityFeedbackCard>(this, "on-close-submit-digital-feedback", (sender) =>
            {
                MessagingCenter.Send(this, "on-digital-content-learning-closed");
                _viewModel.IsBusy = true;
                NavigationService.GoBack();
            });

            digitalContentPlayerFullscreenHandler.Init();
        }

        ~MyDigitalContentLearningView()
        {
            MessagingCenter.Unsubscribe<LearningOpportunityFeedbackCard>(this, "on-close-submit-digital-feedback");
        }

        private void Closed_Tapped(object sender, EventArgs e)
        {
            if (!_viewModel.IsBusy)
            {
                _viewModel.PlayerData?.Close();
                MessagingCenter.Send(this, "on-digital-content-learning-closed");
                _viewModel.IsBusy = true;
                NavigationService.GoBack();
            }
        }
    }
}
