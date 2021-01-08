using System;
using Android.App;
using Android.Content;

namespace LearnerApp.Droid
{
    public interface IMainActivityWithStarting
    {
        void StartActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback);
    }
}
