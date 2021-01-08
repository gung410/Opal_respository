using System;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public partial class ConfirmPopup
    {
        public EventHandler<bool> ConfirmedEventHandle;

        public ConfirmPopup(string message, string cancelTextBtn, string confirmedTextBtn)
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                MessageLbl.Text = message;
                CancelBtn.Text = cancelTextBtn;
                ConfirmBtn.Text = confirmedTextBtn;
            });
        }

        private void Closed_Tapped(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.RemovePageAsync(this);
            }

            ConfirmedEventHandle?.Invoke(this, false);
        }

        private void Confirmed_Tapped(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.RemovePageAsync(this);
            }

            ConfirmedEventHandle?.Invoke(this, true);
        }
    }
}
