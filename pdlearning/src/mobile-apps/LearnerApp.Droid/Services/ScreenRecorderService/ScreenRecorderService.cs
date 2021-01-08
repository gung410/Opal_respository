using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using LearnerApp.Droid.Services.ScreenRecorderService;
using LearnerApp.Helper;
using LearnerApp.Plugins.ScreenRecorder;
using LearnerApp.Services.Download;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ScreenRecorderService))]

namespace LearnerApp.Droid.Services.ScreenRecorderService
{
    public class ScreenRecorderService : IScreenRecorderService
    {
        public const int RequestMediaProjection = 1;

        public ScreenRecorderManager Manager;
        public MediaProjectionManager _mediaProjectionManager;
        public MediaProjection _mediaProjection;
        public MediaRecorder _mediaRecorder;
        public VirtualDisplay _mVirtualDisplay;
        public string FilePath;
        public Action<string> OnRecordTimeout;
        private Timer _currentTimer;

        public ScreenRecorderService()
        {
            Current = this;
        }

        internal static ScreenRecorderService Current { get; private set; }

        public Task StartRecording(ScreenRecorderManager manager, string fileName, Action<string> onRecordTimeout)
        {
            OnRecordTimeout = onRecordTimeout;
            Manager = manager;
            FilePath = FilePathHelper.GetFolderFilePath(FilePathHelper.RecordingPath, fileName + ".mp4");
            CrossCurrentActivity.Current.Activity.StartActivity(new Intent(
                CrossCurrentActivity.Current.Activity,
                typeof(ScreenRecorderActivity)));

            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch
                {
                    Debug.WriteLine("Unable to remove file");
                }
            }

            return Task.CompletedTask;
        }

        public Task<string> StopRecording()
        {
            CrossCurrentActivity.Current.Activity.StopService(
                new Intent(CrossCurrentActivity.Current.Activity, typeof(ScreenRecorderFloatingService)));

            if (_mediaRecorder != null)
            {
                _mediaRecorder.Stop();
                _mediaRecorder.Reset();
                _mediaRecorder.Release();
                _mVirtualDisplay.Release();

                _currentTimer.Dispose();

                _currentTimer = null;
                OnRecordTimeout = null;
                _mediaProjectionManager = null;
                _mediaProjection = null;
                _mediaRecorder = null;
                _mVirtualDisplay = null;
            }

            var result = Task.FromResult(FilePath);
            FilePath = null;

            return result;
        }

        public Task<bool> IsRecording()
        {
            return Task.FromResult(_mediaProjectionManager != null);
        }

        public void StartWatch()
        {
            _currentTimer = null;
            _currentTimer = new System.Threading.Timer(
                o =>
                {
                    var onRecordTimeout = OnRecordTimeout;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var filePath = await StopRecording();
                        onRecordTimeout.Invoke(filePath);
                    });
                },
                null,
                TimeSpan.FromMinutes(10),
                TimeSpan.FromMilliseconds(-1));
        }
    }
}
