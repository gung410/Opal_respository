using System;
using System.Threading.Tasks;
using LearnerApp.Models.Learner;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningCourseBookmarked : ContentView
    {
        public EventHandler BackEventHandler;

        private readonly MyLearningCourseBookmarkViewModel _viewModel;

        public MyLearningCourseBookmarked()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningCourseBookmarkViewModel();
        }

        public async Task LoadCourseBookmarked(BookmarkType bookmarkType)
        {
            await _viewModel.LoadCourseBookmarkCollection(bookmarkType);
        }

        private void BackButton_Tapped(object sender, System.EventArgs e)
        {
            BackEventHandler?.Invoke(this, null);
        }
    }
}
