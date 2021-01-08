using System.Collections.ObjectModel;
using LearnerApp.Models.Course;
using LearnerApp.ViewModels;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class WithdrawalReasonView
    {
        private WithdrawalReasonViewModel _viewModel;

        public WithdrawalReasonView()
        {
            InitializeComponent();

            _viewModel = (WithdrawalReasonViewModel)BindingContext;
        }

        private void WithdrawalReasonCollection_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var view = sender as CollectionView;

            if (view?.ItemsSource == null)
            {
                return;
            }

            int currentCount = ((ObservableCollection<Comment>)view.ItemsSource).Count;

            if (currentCount < _viewModel.TotalCount && e.LastVisibleItemIndex == currentCount - 1)
            {
                _viewModel.LoadMoreItemCommand.Execute(null);
            }
        }
    }
}
