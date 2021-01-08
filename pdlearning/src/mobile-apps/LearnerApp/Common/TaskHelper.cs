using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LearnerApp.Common
{
    public static class TaskHelper
    {
        public static Task RunSequential(IEnumerator<Func<Task>> actions, Action onComplete = null, Action<Exception> onError = null)
        {
            if (!actions.MoveNext())
            {
                onComplete?.Invoke();
            }

            if (actions.Current == null)
            {
                return Task.CompletedTask;
            }

            var task = actions.Current();
            task.ContinueWith(t => onError?.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => RunSequential(actions, onComplete, onError), TaskContinuationOptions.OnlyOnRanToCompletion);

            return task;
        }
    }
}
