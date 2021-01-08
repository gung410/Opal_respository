using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Webkit;
using Java.IO;
using LearnerApp.Common;
using Plugin.CurrentActivity;
using Plugin.HybridWebView.Droid;
using Plugin.Media;

namespace LearnerApp.Droid.Renderers
{
    public class CustomHybridWebViewChromeClient : HybridWebViewChromeClient
    {
        private static string externalContentUri;

        private IValueCallback uploadMessage;

        public CustomHybridWebViewChromeClient(HybridWebViewRenderer renderer) : base(renderer)
        {
        }

        private Context CurrentContext => CrossCurrentActivity.Current.Activity;

        public override bool OnShowFileChooser(Android.Webkit.WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            var appActivity = CurrentContext as MainActivity;
            uploadMessage = filePathCallback;

            string[] items = { "Take Photo", "File Browser", "Cancel" };

            using (var dialogBuilder = new AlertDialog.Builder(CurrentContext))
            {
                dialogBuilder.SetItems(items, async (d, args) =>
                {
                    Intent intent = new Intent();
                    int requestCode = args.Which;

                    switch (requestCode)
                    {
                        case (int)ResultCodeIntent.TakePhotoResultCode:
                            externalContentUri = await TakePhoto();
                            break;
                        case (int)ResultCodeIntent.FileChooserResultCode:
                            intent = fileChooserParams.CreateIntent();
                            break;
                    }

                    appActivity.StartActivity(intent, requestCode, OnActivityResult);
                });

                // Prevent dissmiss dialog when click back button and click outside
                // We must prevent because value null cause nullpointer expeption
                dialogBuilder.SetCancelable(false);
                dialogBuilder.Show();
            }

            return true;
        }

        /*
         * below lines for fixing issue #66: Not support HTML Input File
         * reference this workaround: https://forums.xamarin.com/discussion/62284/input-type-file-doesnt-work-on-xamarin-forms-webview-androidhome.firefoxchina.cn
         */
        private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (uploadMessage == null)
            {
                return;
            }

            if (requestCode == (int)ResultCodeIntent.FileChooserResultCode)
            {
                uploadMessage.OnReceiveValue(WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data));
            }
            else if (requestCode == (int)ResultCodeIntent.TakePhotoResultCode)
            {
                // We get data from taken a photo
                var uri = GetResultData(externalContentUri);
                uploadMessage.OnReceiveValue(uri);
            }
            else
            {
                uploadMessage.OnReceiveValue(null);
            }

            uploadMessage = null;
        }

        private Uri[] GetResultData(string imageUri)
        {
            Uri[] results = null;

            if (!string.IsNullOrEmpty(imageUri))
            {
                results = new Uri[] { Uri.FromFile(new File(imageUri)) };
            }

            return results;
        }

        private async Task<string> TakePhoto()
        {
            string path = string.Empty;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return path;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "ProfilePicture",
                Name = "profilePicture.jpg"
            });

            if (file == null)
            {
                return path;
            }

            return file.Path;
        }
    }
}
