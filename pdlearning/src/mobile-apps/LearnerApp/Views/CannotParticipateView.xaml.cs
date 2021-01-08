using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CannotParticipateView
    {
        public CannotParticipateView()
        {
            InitializeComponent();

            ConfirmBtn.IsEnabled = !string.IsNullOrEmpty(Reason.Text);
        }

        private void InputView_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ConfirmBtn.IsEnabled = !string.IsNullOrEmpty(Reason.Text);
        }
    }
}
