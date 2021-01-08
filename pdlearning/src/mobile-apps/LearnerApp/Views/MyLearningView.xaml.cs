using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class MyLearningView
    {
        public MyLearningView()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() => true;
    }
}
