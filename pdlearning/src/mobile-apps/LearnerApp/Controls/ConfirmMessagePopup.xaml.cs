using System;
using System.Linq;
using LearnerApp.Common;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public partial class ConfirmMessagePopup
    {
        public EventHandler<ConfirmMessage> CustomEventHandler;

        private readonly bool _checkValidate;

        public ConfirmMessagePopup(string title, string cancelTextBtn, string confirmedTextBtn, bool checkValidate)
        {
            InitializeComponent();

            _checkValidate = checkValidate;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (checkValidate)
                {
                    ConfirmFrame.IsEnabled = false;
                    ConfirmBtn.TextColor = Color.LightSlateGray;
                }

                MessageLbl.Text = title;
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

            ConfirmMessage confirmMessage = new ConfirmMessage { IsConfirm = false, Reason = Reason.Text };

            CustomEventHandler?.Invoke(this, confirmMessage);
        }

        private void Confirmed_Tapped(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.RemovePageAsync(this);
            }

            ConfirmMessage confirmMessage = new ConfirmMessage { IsConfirm = true, Reason = Reason.Text };

            CustomEventHandler?.Invoke(this, confirmMessage);
        }

        private void Reason_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_checkValidate)
            {
                return;
            }

            ReasonFrame.BorderColor = string.IsNullOrEmpty(Reason.Text) ? Color.Red : Color.LightGray;
            ConfirmFrame.IsEnabled = !string.IsNullOrEmpty(Reason.Text);
            ConfirmBtn.TextColor = string.IsNullOrEmpty(Reason.Text) ? Color.LightSlateGray : Color.White;
        }
    }
}
