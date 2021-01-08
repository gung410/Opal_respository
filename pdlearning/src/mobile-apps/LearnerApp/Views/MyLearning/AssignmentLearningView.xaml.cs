using System;
using LearnerApp.Controls.DigitalContentPlayer;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssignmentLearningView
    {
        public AssignmentLearningView()
        {
            InitializeComponent();
            var digitalContentPlayerFullscreenHandler = new DigitalContentPlayerFullscreenHandler(
                this,
                new VisualElement[] { ContentMetadataView });

            digitalContentPlayerFullscreenHandler.Init();
        }

        private AssignmentLearningViewModel ViewModel => (AssignmentLearningViewModel)BindingContext;

        private void OnClose_Tapped(object sender, EventArgs e)
        {
            ViewModel.CloseAssignmentLearning();
        }
    }
}
