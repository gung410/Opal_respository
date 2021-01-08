using System;
using LearnerApp.Models;
using LearnerApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningPathsCreateNewView
    {
        private MyLearningPathsCreateNewViewModel _viewModel;

        public MyLearningPathsCreateNewView()
        {
            InitializeComponent();

            _viewModel = BindingContext as MyLearningPathsCreateNewViewModel;
        }

        private void RemoveCourse_Clicked(object sender, EventArgs e)
        {
            var item = (e as TappedEventArgs).Parameter as ItemCard;

            _viewModel.OnRemoveCourseItem(item);
        }
    }
}
