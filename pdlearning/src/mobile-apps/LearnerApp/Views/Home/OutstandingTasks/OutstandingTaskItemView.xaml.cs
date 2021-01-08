using System;
using Plugin.Toast;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Home.OutstandingTasks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutstandingTaskItemView
    {
        public OutstandingTaskItemView()
        {
            InitializeComponent();
        }

        private void CanNotStart_ButtonClicked(object sender, EventArgs e)
        {
            CrossToastPopUp.Current.ShowCustomToast(
                "Unable to start because it’s not yet reached the start date",
                "#8799ba",
                "#FCFDFD");
        }
    }
}
