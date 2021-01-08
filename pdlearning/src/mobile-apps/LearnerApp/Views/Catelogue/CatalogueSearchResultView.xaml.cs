using System;
using LearnerApp.ViewModels.Catalogue;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Views.Catelogue
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CatalogueSearchResultView : ContentView
    {
        private CatalogueSearchResultViewModel _viewModel;

        public CatalogueSearchResultView()
        {
            InitializeComponent();

            BindingContext = _viewModel = new CatalogueSearchResultViewModel();
        }

        public event EventHandler<bool> OnDismissSearch;

        private void OnCatalogueSearch_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.Keyword))
            {
                return;
            }

            OnDismissSearch?.Invoke(this, false);
        }

        private void OnDismissSearch_Clicked(object sender, System.EventArgs e)
        {
            _viewModel.Keyword = string.Empty;
        }

        private void KeywordInput_Completed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.Keyword))
            {
                return;
            }

            OnDismissSearch?.Invoke(this, false);
        }

        private void KeywordInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.Keyword))
            {
                OnDismissSearch?.Invoke(this, true);
            }
        }

        private void OnPdoSearch_Clicked(object sender, EventArgs e)
        {
            PdoSearchBtn.BackgroundColor = Color.LightGray;
            UserSearchBtn.BackgroundColor = Color.White;
        }

        private void OnUserSearch_Clicked(object sender, EventArgs e)
        {
            UserSearchBtn.BackgroundColor = Color.LightGray;
            PdoSearchBtn.BackgroundColor = Color.White;
        }
    }
}
