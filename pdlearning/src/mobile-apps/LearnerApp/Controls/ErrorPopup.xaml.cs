using System;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public partial class ErrorPopup
    {
        public EventHandler<bool> CloseEventHandler;

        public ErrorPopup(string message, string cancelTextBtn, bool isVisibleIcon)
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                MessageLbl.Text = message;
                CancelBtn.Text = cancelTextBtn;
                PopupIcon.IsVisible = isVisibleIcon;
            });
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAsync();
            }

            CloseEventHandler?.Invoke(this, true);
        }
    }
}
