using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningPaths : ContentView
    {
        private MyLearningPathsViewModel _viewModel;

        public MyLearningPaths()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningPathsViewModel();
        }

        public void Init()
        {
            _viewModel.InitLearningPaths();
        }
    }
}
