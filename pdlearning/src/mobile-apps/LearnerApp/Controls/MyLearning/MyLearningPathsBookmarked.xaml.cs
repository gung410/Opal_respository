using System;
using System.Threading.Tasks;
using LearnerApp.Models.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningPathsBookmarked : ContentView
    {
        public EventHandler BackEventHandler;
        private readonly MyLearningPathsBookmarkedViewModel _viewModel;

        public MyLearningPathsBookmarked()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningPathsBookmarkedViewModel();
        }

        public async Task LoadLearningPathsBookmarked()
        {
            await _viewModel.LoadLearningPathsBookmarkCollection(false);
        }

        private void BackButton_Tapped(object sender, EventArgs e)
        {
            BackEventHandler?.Invoke(this, null);
        }
    }
}
