using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Java.Lang;
using Plugin.CurrentActivity;
using Debug = System.Diagnostics.Debug;

namespace LearnerApp.Droid.Services.ScreenRecorderService
{
    [Activity(Label = "Screen recording", Theme = "@style/MainTheme")]
    public class ScreenRecorderActivity : AppCompatActivity
    {
        private static int _codeDrawOverOtherAppPermission = 2084;

        private int _displayWidth = 1080;
        private int _displayHeight = 1920;
        private DisplayMetrics _metrics;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(_metrics);

            if (Settings.CanDrawOverlays(this))
            {
                StartRequestingCaptureScreen();
            }
            else
            {
                Intent intent = new Intent(
                    Settings.ActionManageOverlayPermission,
                    Uri.Parse("package:" + PackageName));

                StartActivityForResult(intent, _codeDrawOverOtherAppPermission);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == _codeDrawOverOtherAppPermission)
            {
                HandleOverlayPermission(requestCode, resultCode, data);
            }

            if (requestCode == ScreenRecorderService.RequestMediaProjection)
            {
                HandleRecordScreenPermission(requestCode, resultCode, data);
            }
        }

        private void StartRequestingCaptureScreen()
        {
            ScreenRecorderService.Current._mediaProjectionManager =
                MainApplication.CurrentContext.GetSystemService(Context.MediaProjectionService) as
                    MediaProjectionManager;

            if (ScreenRecorderService.Current._mediaProjectionManager == null)
            {
                return;
            }

            StartActivityForResult(
                ScreenRecorderService.Current._mediaProjectionManager.CreateScreenCaptureIntent(),
                ScreenRecorderService.RequestMediaProjection);
        }

        private void HandleOverlayPermission(in int requestCode, Result resultCode, Intent data)
        {
            if (Settings.CanDrawOverlays(this))
            {
                StartRequestingCaptureScreen();
            }
            else
            {
                Finish();
            }
        }

        private void InitializeOverlayView()
        {
            StartService(new Intent(
                this, typeof(ScreenRecorderFloatingService)));
        }

        private void HandleRecordScreenPermission(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok)
            {
                Debug.WriteLine("User does not grant permission");
                Finish();
                return;
            }

            InitializeOverlayView();
            var screenDensity = (int)_metrics.DensityDpi;

            ScreenRecorderService.Current._mediaRecorder = new MediaRecorder();
            InitRecorder();
            PrepareRecorder();
            ScreenRecorderService.Current._mediaProjection = ScreenRecorderService.Current._mediaProjectionManager.GetMediaProjection((int)resultCode, data);
            ScreenRecorderService.Current._mediaProjection.RegisterCallback(new MediaProjectionCallBack(this), null);
            ScreenRecorderService.Current._mVirtualDisplay = ScreenRecorderService.Current._mediaProjection.CreateVirtualDisplay(
                ScreenRecorderService.Current.FilePath,
                _displayWidth,
                _displayHeight,
                screenDensity,
                DisplayFlags.Presentation,
                ScreenRecorderService.Current._mediaRecorder.Surface,
                null,
                null);

            ScreenRecorderService.Current._mediaRecorder.Start();
            ScreenRecorderService.Current.StartWatch();
            Finish();
        }

        private void PrepareRecorder()
        {
            try
            {
                ScreenRecorderService.Current._mediaRecorder.Prepare();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

        private void InitRecorder()
        {
            var filePath = ScreenRecorderService.Current.FilePath;
            ScreenRecorderService.Current._mediaRecorder.SetVideoSource(VideoSource.Surface);
            ScreenRecorderService.Current._mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            ScreenRecorderService.Current._mediaRecorder.SetVideoEncoder(VideoEncoder.H264);
            ScreenRecorderService.Current._mediaRecorder.SetVideoEncodingBitRate(512 * 1000);
            ScreenRecorderService.Current._mediaRecorder.SetVideoFrameRate(30);
            ScreenRecorderService.Current._mediaRecorder.SetVideoSize(_displayWidth, _displayHeight);
            ScreenRecorderService.Current._mediaRecorder.SetOutputFile(filePath);
        }

        public class MediaProjectionCallBack : MediaProjection.Callback
        {
            private readonly ScreenRecorderActivity _screenRecorder;

            public MediaProjectionCallBack(ScreenRecorderActivity screenRecorder)
            {
                _screenRecorder = screenRecorder;
            }

            public override void OnStop()
            {
                base.OnStop();
            }
        }
    }
}
