using System;
using System.Linq;
using LearnerApp.Models.UserOnBoarding;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LearnerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyClassRunRejectReasonPopup
    {
        public MyClassRunRejectReasonPopup(UserInformation userInfo, DateTime changedDate, string reason)
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                if (userInfo != null)
                {
                    if (!string.IsNullOrEmpty(userInfo.AvatarUrl))
                    {
                        AvatarImg.Source = userInfo.AvatarUrl;
                    }
                    else
                    {
                        AvatarImg.Source = "default_avatar.png";
                    }

                    FullNameLbl.Text = userInfo.FullName;
                }

                ChangeDateLbl.Text = changedDate.ToString("dd/MM/yyyy");
                ReasonLbl.Text = reason;
            });
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Any())
            {
                PopupNavigation.Instance.PopAllAsync();
            }
        }
    }
}
