using System;
using System.Linq;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace LearnerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInView
    {
        public CheckInView()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);

            var permission = response.FirstOrDefault(res => res.Key == Permission.Camera && res.Value == PermissionStatus.Granted);

            if (permission.Key.Equals(default(Permission)) && permission.Value.Equals(default(PermissionStatus)))
            {
                return;
            }

            var scan = new ZXingScannerPage();
            await Navigation.PushAsync(scan);
            scan.OnScanResult += async (result) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Navigation.PopAsync();
                    SessionCode.Text = result.Text;
                });
            };
        }
    }
}
