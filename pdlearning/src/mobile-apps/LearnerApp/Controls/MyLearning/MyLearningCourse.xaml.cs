using System.Threading.Tasks;
using LearnerApp.Models.MyLearning;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Controls.MyLearning
{
    public partial class MyLearningCourse : ContentView
    {
        private readonly MyLearningCourseViewModel _viewModel;

        public MyLearningCourse()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningCourseViewModel();
        }

        public async Task Init()
        {
            await _viewModel.InitScreen(MyLearningStatus.Registered);
        }
    }
}
