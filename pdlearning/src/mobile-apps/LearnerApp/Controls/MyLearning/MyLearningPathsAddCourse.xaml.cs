using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.ViewModels.MyLearning;
using Plugin.Toast;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls.MyLearning
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyLearningPathsAddCourse
    {
        public EventHandler<List<ItemCard>> DoneEventHandler;
        public MyLearningPathsAddCourseViewModel _viewModel;

        public MyLearningPathsAddCourse(IEnumerable<ItemCard> courseSelectedCollection)
        {
            InitializeComponent();

            BindingContext = _viewModel = new MyLearningPathsAddCourseViewModel(courseSelectedCollection);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.GetNewlyAddedCollection();
        }

        private void Done_Clicked(object sender, EventArgs e)
        {
            if (_viewModel.CourseSelected.IsNullOrEmpty())
            {
                CrossToastPopUp.Current.ShowToastWarning("Please select at least 1 course");

                return;
            }

            DoneEventHandler?.Invoke(this, _viewModel.CourseSelected);

            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAllAsync();
            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAllAsync();
            }
        }

        private async void SourceList_Scrolled(object sender, Xamarin.Forms.ItemsViewScrolledEventArgs e)
        {
            var view = sender as CollectionView;

            if (view.ItemsSource == null)
            {
                return;
            }

            int currentCount = (view.ItemsSource as ObservableCollection<ItemCard>).Count;

            if (currentCount < _viewModel.TotalCount && e.LastVisibleItemIndex == currentCount - 1)
            {
                await _viewModel.OnLoadmore();
            }
        }
    }
}
