using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Java.Lang;

namespace LearnerApp.Droid.Services.ScreenRecorderService
{
    [Service(Enabled = true, Exported = true)]
    public class ScreenRecorderFloatingService : Service
    {
        private IWindowManager _windowManager;
        private View _floatingView;
        private WindowManagerLayoutParams _params;

        public ScreenRecorderFloatingService()
        {
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.stop_recording_layout, null);
            var pulesView = _floatingView.FindViewById<View>(Resource.Id.pulse_view);

            var anim = AnimationUtils.LoadAnimation(this, Resource.Animation.pulse_anim);
            pulesView.StartAnimation(anim);

            WindowManagerLayoutParams param = new WindowManagerLayoutParams(
                WindowManagerLayoutParams.WrapContent,
                WindowManagerLayoutParams.WrapContent,
                WindowManagerTypes.ApplicationOverlay,
                WindowManagerFlags.NotFocusable,
                Android.Graphics.Format.Translucent);

            param.Gravity = GravityFlags.Bottom | GravityFlags.Right;
            param.X = 50;
            param.Y = 200;

            _windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            _windowManager.AddView(_floatingView, param);
            _params = param;

            _floatingView.SetOnTouchListener(new OnTouchListener(this));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_floatingView != null)
            {
                _windowManager.RemoveView(_floatingView);
            }
        }

        private class OnTouchListener : Java.Lang.Object, View.IOnTouchListener
        {
            private readonly ScreenRecorderFloatingService _owner;
            private DateTime _lastClickDateTime = DateTime.MaxValue;
            private int _initialX;
            private int _initialY;
            private float _initialTouchX;
            private float _initialTouchY;

            public OnTouchListener(ScreenRecorderFloatingService screenRecorderFloatingService)
            {
                _owner = screenRecorderFloatingService;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        _lastClickDateTime = DateTime.Now;
                        _initialX = _owner._params.X;
                        _initialY = _owner._params.Y;

                        _initialTouchX = e.RawX;
                        _initialTouchY = e.RawY;

                        return true;
                    case MotionEventActions.Up:
                        if ((DateTime.Now - _lastClickDateTime).TotalMilliseconds <= 200)
                        {
                            ScreenRecorderService.Current.Manager.StopRecording();
                        }

                        _lastClickDateTime = DateTime.MinValue;
                        return true;
                    case MotionEventActions.Move:
                        _owner._params.X = _initialX - (int)(e.RawX - _initialTouchX);
                        _owner._params.Y = _initialY - (int)(e.RawY - _initialTouchY);

                        _owner._windowManager.UpdateViewLayout(_owner._floatingView, _owner._params);
                        return true;
                }

                return false;
            }
        }
    }
}
