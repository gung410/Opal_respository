using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AVFoundation;
using Cirrious.FluentLayouts.Touch;
using CoreFoundation;
using Foundation;
using LearnerApp.Helper;
using LearnerApp.iOS.Helpers.Spring;
using LearnerApp.iOS.Services;
using LearnerApp.Plugins.ScreenRecorder;
using ReplayKit;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ScreenRecorderService))]

namespace LearnerApp.iOS.Services
{
    public class ScreenRecorderService : IScreenRecorderService
    {
        private AVAssetWriter _videoWriter;
        private string _path;
        private AVAssetWriterInput _videoWriterInput;
        private UIView _stopView;
        private UITapGestureRecognizer _gestureRecognizerHandler;
        private ScreenRecorderManager _manager;
        private SpringViewWrapper _springAnim;
        private Action<string> _onRecordTimeout;
        private Timer _currentTimer;
        private UIPanGestureRecognizer _dragAndDropRecognizerHandler;

        public async Task StartRecording(ScreenRecorderManager manager, string fileName, Action<string> onRecordTimeout)
        {
            if (!RPScreenRecorder.SharedRecorder.Available)
            {
                return;
            }

            await Cleanup();
            _manager = manager;
            _path = FilePathHelper.GetFolderFilePath(FilePathHelper.RecordingPath, fileName + ".mp4");
            _onRecordTimeout = onRecordTimeout;

            try
            {
                File.Delete(_path);
            }
            catch
            {
                // Do nothing
            }

            _videoWriterInput = new AVAssetWriterInput(
                AVMediaType.Video, new AVVideoSettingsCompressed()
                {
                    Codec = AVVideoCodec.H264,
                    Width = (int)UIScreen.MainScreen.Bounds.Width * 2,
                    Height = (int)UIScreen.MainScreen.Bounds.Height * 2 // Full hd
                });
            _videoWriter = new AVAssetWriter(NSUrl.FromFilename(_path), AVFileType.Mpeg4, out NSError error);
            _videoWriter.AddInput(_videoWriterInput);

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                RPScreenRecorder.SharedRecorder.StartCapture(
                    (buffer, type, nsError) =>
                    {
                        try
                        {
                            if (nsError != null)
                            {
                                Debug.WriteLine(nsError.ToString());
                                return;
                            }

                            switch (type)
                            {
                                case RPSampleBufferType.Video:
                                    if (_videoWriter.Status == AVAssetWriterStatus.Unknown)
                                    {
                                        _videoWriter.StartWriting();
                                        _videoWriter.StartSessionAtSourceTime(buffer.PresentationTimeStamp);
                                    }

                                    if (_videoWriter.Status == AVAssetWriterStatus.Writing)
                                    {
                                        if (_videoWriterInput.ReadyForMoreMediaData)
                                        {
                                            _videoWriterInput.AppendSampleBuffer(buffer);
                                        }
                                    }

                                    break;
                            }
                        }
                        finally
                        {
                            buffer.Dispose();
                        }
                    },
                    nsError =>
                    {
                        AddStopViewToKeyWindow();
                        StartWatch();
                    });
            });
        }

        public async Task<string> StopRecording()
        {
            await Cleanup();
            var path = _path;
            return path;
        }

        public Task<bool> IsRecording()
        {
            return Task.FromResult(_stopView != null);
        }

        private async Task Cleanup()
        {
            _manager = null;
            _springAnim = null;
            _onRecordTimeout = null;

            _currentTimer?.Dispose();
            _currentTimer = null;

            if (_stopView != null)
            {
                _stopView.RemoveGestureRecognizer(_gestureRecognizerHandler);
                _stopView.RemoveGestureRecognizer(_dragAndDropRecognizerHandler);
                _gestureRecognizerHandler = null;
                _dragAndDropRecognizerHandler = null;
                _stopView.RemoveFromSuperview();
                _stopView = null;
            }

            if (!RPScreenRecorder.SharedRecorder.Available || _videoWriter == null)
            {
                return;
            }

            var tcs = new TaskCompletionSource<object>();
            RPScreenRecorder.SharedRecorder.StopCapture((nsError) =>
            {
                if (nsError != null)
                {
                    Debug.WriteLine(nsError.ToString());
                }

                if (_videoWriter != null)
                {
                    _videoWriterInput.MarkAsFinished();
                    _videoWriter.FinishWriting(() =>
                    {
                        _videoWriter = null;
                        _videoWriterInput = null;
                        tcs.SetResult(null);
                    });
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            await tcs.Task;
        }

        private void StartWatch()
        {
            _currentTimer = null;
            _currentTimer = new System.Threading.Timer(
                o =>
                {
                    var onRecordTimeout = _onRecordTimeout;
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

        private void AddStopViewToKeyWindow()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var view = new UIView();

            view.BackgroundColor = UIColor.FromRGB(231, 76, 60);

            // Add red round view
            window.AddSubview(view);
            view.TranslatesAutoresizingMaskIntoConstraints = false;

            var trailingConstraint = view.AtTrailingOf(window, 25);
            var bottomConstraint = view.AtBottomOf(window, 125);

            window.AddConstraints(
                trailingConstraint,
                bottomConstraint);
            var viewSize = 75.0f;

            view.AddConstraints(
                view.Height().EqualTo(viewSize),
                view.Width().EqualTo(viewSize));
            view.Layer.CornerRadius = viewSize / 2;

            AddPulseViewToView(view);
            AddStopLabelToView(view);

            _stopView = view;
            _gestureRecognizerHandler = new UITapGestureRecognizer(async () =>
            {
                await _manager.StopRecording();
            });

            nfloat originalX = 0;
            nfloat originalY = 0;

            _dragAndDropRecognizerHandler = new UIPanGestureRecognizer(recognizer =>
            {
                var translation = recognizer.TranslationInView(_stopView.Superview);

                if (recognizer.State == UIGestureRecognizerState.Began)
                {
                    originalX = trailingConstraint.Constant;
                    originalY = bottomConstraint.Constant;
                    return;
                }

                if (recognizer.State != UIGestureRecognizerState.Cancelled)
                {
                    trailingConstraint.Constant = originalX + translation.X;
                    bottomConstraint.Constant = originalY + translation.Y;
                }
            });

            _stopView.AddGestureRecognizer(_gestureRecognizerHandler);
            _stopView.AddGestureRecognizer(_dragAndDropRecognizerHandler);
        }

        private void AddPulseViewToView(UIView view)
        {
            var pulseView = new UIView();
            pulseView.BackgroundColor = UIColor.FromRGB(231, 76, 60).ColorWithAlpha(0.5f);
            pulseView.TranslatesAutoresizingMaskIntoConstraints = false;
            pulseView.Layer.CornerRadius = 75.0f / 2;
            pulseView.ClipsToBounds = false;

            view.InsertSubview(pulseView, 0);
            view.AddConstraints(pulseView.FullSizeOf(view));

            _springAnim = new SpringViewWrapper(pulseView)
            {
                Animation = Spring.AnimationPreset.Pop,
                RepeatCount = float.MaxValue,
                Duration = 2
            };

            _springAnim.Animate();
        }

        private void AddStopLabelToView(UIView view)
        {
            var uiLabel = new UILabel();
            uiLabel.Text = "Stop";
            uiLabel.TextColor = UIColor.White;
            uiLabel.TextAlignment = UITextAlignment.Center;
            uiLabel.TranslatesAutoresizingMaskIntoConstraints = false;

            view.AddSubview(uiLabel);
            view.AddConstraints(uiLabel.FullSizeOf(view));
        }
    }
}
