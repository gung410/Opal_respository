using System;
using System.Collections.Generic;
using System.Linq;
using LearnerApp.Models.MyLearning;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrokenLinkReportPopup
    {
        public EventHandler<BrokenLinkReport> OnConfirmedEventHandler;

        private string _linkSelected;

        public BrokenLinkReportPopup(List<string> urls)
        {
            InitializeComponent();

            LinkCollection.ItemsSource = urls.Distinct().ToList();
        }

        private void OnClosed_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    PopupNavigation.Instance.RemovePageAsync(this);
                }
            });
        }

        private void OnConfirmed_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_linkSelected))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SelectTheLinkFrame.BorderColor = Color.Red;
                });

                return;
            }

            OnConfirmedEventHandler?.Invoke(this, new BrokenLinkReport { Link = _linkSelected, Description = BrokenLinkDescription.Text });

            Device.BeginInvokeOnMainThread(() =>
            {
                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    PopupNavigation.Instance.RemovePageAsync(this);
                }
            });
        }

        private void BrokenLinkSelect_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DataStack.IsVisible = false;
                CollectionStack.IsVisible = true;
            });
        }

        private void LinkCollection_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var view = sender as ListView;

            if (view.SelectedItem == null)
            {
                return;
            }

            _linkSelected = e.SelectedItem as string;

            Device.BeginInvokeOnMainThread(() =>
            {
                LinkSelectedLbl.Text = _linkSelected;
                SelectTheLinkFrame.BorderColor = Color.LightGray;
                LinkSelectedLbl.TextColor = Color.Black;

                CollectionStack.IsVisible = false;
                DataStack.IsVisible = true;
            });

            view.SelectedItem = null;
        }

        private void BackButton_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                CollectionStack.IsVisible = false;
                DataStack.IsVisible = true;
            });
        }
    }
}
