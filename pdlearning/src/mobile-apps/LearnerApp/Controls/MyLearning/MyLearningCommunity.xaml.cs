using System.Threading.Tasks;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Controls.MyLearning
{
    public partial class MyLearningCommunity : ContentView
    {
        private readonly MyLearningCommunityViewModel _viewModel;

        public MyLearningCommunity()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningCommunityViewModel();
        }

        public async Task Init()
        {
            await _viewModel.GetCommunitiesWithoutKeyword(false);
        }
    }
}
