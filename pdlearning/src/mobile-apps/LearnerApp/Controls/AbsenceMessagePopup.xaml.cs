using System;
using System.IO;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace LearnerApp.Controls
{
    public partial class AbsenceMessagePopup
    {
        public EventHandler<bool> CustomEventHandler;

        public AbsenceMessagePopup(string title, string reason, string attachment, string cancelTextBtn)
        {
            InitializeComponent();

            string fileName = string.Empty;

            if (IsValidUrl(attachment))
            {
                Uri uri = new Uri(attachment);
                fileName = Path.GetFileName(uri.LocalPath);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                MessageLbl.Text = title;
                Reason.Text = reason;
                AttachmentLbl.Text = fileName;
                AttachmentFileName.IsVisible = !string.IsNullOrEmpty(fileName);
                CancelBtn.Text = cancelTextBtn;
            });
        }

        private void Closed_Tapped(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.RemovePageAsync(this);
            }

            CustomEventHandler?.Invoke(this, false);
        }

        private bool IsValidUrl(string url)
        {
            Uri outUri;

            return Uri.TryCreate(url, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
