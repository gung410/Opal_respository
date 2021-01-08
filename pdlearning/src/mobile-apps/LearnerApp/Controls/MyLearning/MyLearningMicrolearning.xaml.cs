using System.Threading.Tasks;
using LearnerApp.Models.MyLearning;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningMicrolearning : ContentView
    {
        private readonly MyLearningMicrolearningViewModel _viewModel;

        public MyLearningMicrolearning()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningMicrolearningViewModel();
        }

        public async Task Init()
        {
            await _viewModel.InitScreen(MyLearningStatus.InProgress);
        }
    }
}
