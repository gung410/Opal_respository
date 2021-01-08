using System;
using LearnerApp.Models.UserOnBoarding;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShareMyLearningPathsView
    {
        private ShareMyLearningPathsViewModel _viewModel;

        public ShareMyLearningPathsView()
        {
            InitializeComponent();

            _viewModel = (ShareMyLearningPathsViewModel)BindingContext;
        }

        private void RemoveCourse_Clicked(object sender, EventArgs e)
        {
            var item = (e as TappedEventArgs).Parameter as UserInformation;

            _viewModel.OnRemoveShareUser(item);
        }
    }
}
