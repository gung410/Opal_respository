using System;
using System.Threading.Tasks;

namespace LearnerApp.Common
{
    // Define Observer to wait flow in webview prevent crash app
    public class Observer
    {
        private ObeserverState _currentState;
        private object _syncObj = new object();

        public void CallAfter(Func<Task> callback, int afterMilis)
        {
            lock (_syncObj)
            {
                if (_currentState != null)
                {
                    _currentState.IsActive = false;
                }

                _currentState = new ObeserverState(callback, afterMilis);
            }
        }

        private class ObeserverState
        {
            private object _syncObj = new object();
            private bool _isActive = true;

            public ObeserverState(Func<Task> callback, int waitForMilis)
            {
                Callback = callback;
                WaitForMilis = waitForMilis;
                Task.Delay(waitForMilis).ContinueWith(async t =>
                {
                    if (IsActive)
                    {
                        await Callback();
                        IsActive = false;
                    }
                });
            }

            public bool IsActive
            {
                get
                {
                    return _isActive;
                }

                set
                {
                    lock (_syncObj)
                    {
                        _isActive = false;
                    }
                }
            }

            public Func<Task> Callback { get; }

            public int WaitForMilis { get; }
        }
    }
}
