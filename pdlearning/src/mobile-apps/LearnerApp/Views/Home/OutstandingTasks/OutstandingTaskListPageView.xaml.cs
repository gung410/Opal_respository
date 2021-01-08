using System;
using LearnerApp.Common.Helper;
using LearnerApp.ViewModels.Home.OutstandingTasks;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Home.OutstandingTasks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutstandingTaskListPageView
    {
        private readonly StressActionHandler _loadMoreActionHandler = new StressActionHandler();

        public OutstandingTaskListPageView()
        {
            InitializeComponent();
        }

        private OutstandingTaskListPageViewModel ViewModel => (OutstandingTaskListPageViewModel)BindingContext;

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            await _loadMoreActionHandler.RunAsync(async () =>
            {
                await ViewModel.RefreshData();
                RefreshView.IsRefreshing = false;
            });
        }

        private async void SourceCollection_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            await _loadMoreActionHandler.RunAsync(async () =>
            {
                await ViewModel.LoadMore();
            });
        }
    }
}
