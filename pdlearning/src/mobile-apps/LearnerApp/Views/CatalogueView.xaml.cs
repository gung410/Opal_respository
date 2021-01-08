using LearnerApp.ViewModels.Catalogue;
using LearnerApp.Views.Common;
using Xamarin.Forms;

namespace LearnerApp.Views
{
    public partial class CatalogueView : TabRootContentPage
    {
        public CatalogueView()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<CatalogueSearchResultViewModel>(this, "advanced-search-filter-result", sender =>
            {
                CatalogueSession.IsVisible = false;
            });
        }

        ~CatalogueView()
        {
            MessagingCenter.Unsubscribe<CatalogueSearchResultViewModel>(this, "advanced-search-filter-result");
        }

        protected override bool OnBackButtonPressed() => true;

        private void CatalogueSearchResultView_OnDismissSearch(object sender, bool e)
        {
            CatalogueSession.IsVisible = e;
        }
    }
}
