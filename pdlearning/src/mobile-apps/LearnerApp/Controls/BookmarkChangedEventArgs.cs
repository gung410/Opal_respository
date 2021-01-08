using System;

namespace LearnerApp.Controls
{
    public class BookmarkChangedEventArgs : EventArgs
    {
        internal BookmarkChangedEventArgs(object previousValue, object currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        public object PreviousValue { get; }

        public object CurrentValue { get; }
    }
}
