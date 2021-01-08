using System;
using System.Threading.Tasks;
using LearnerApp.Controls.LearnerObservableCollection;
using LearnerApp.Models;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningCommunityBookmarked : ContentView
    {
        public EventHandler BackEventHandler;

        private readonly MyLearningCommunityBookmarkViewModel _viewModel;

        public MyLearningCommunityBookmarked()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningCommunityBookmarkViewModel();
        }

        public async Task LoadCommunityBookmarked()
        {
            _viewModel.CommunityCollection = new LearnerObservableCollection<ItemCard>();
            await _viewModel.LoadCommunityBookmarkCollection();
        }

        private void BackButton_Tapped(object sender, System.EventArgs e)
        {
            BackEventHandler?.Invoke(this, null);
        }
    }
}
