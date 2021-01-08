using System;
using System.Threading.Tasks;
using LearnerApp.Models.Learner;
using LearnerApp.ViewModels.MyLearning;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningBookmarkGroup : ContentView
    {
        public EventHandler<BookmarkType> BookmarkGroupSelectedEventHandler;

        private readonly MyLearningBookmarkGroupViewModel _viewModel;
        private bool _isBusy = false;

        public MyLearningBookmarkGroup()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningBookmarkGroupViewModel();
        }

        public async Task GetMyBookmarkGroupCount()
        {
            await _viewModel.GetBookmarkGroupCount();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            TappedEventArgs agrs = e as TappedEventArgs;

            Enum.TryParse(agrs.Parameter as string, out BookmarkType itemSelected);

            BookmarkGroupSelectedEventHandler?.Invoke(this, itemSelected);

            _isBusy = false;
        }
    }
}
