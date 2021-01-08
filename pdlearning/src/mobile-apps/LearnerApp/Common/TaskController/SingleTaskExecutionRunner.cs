using System;
using System.Threading;
using System.Threading.Tasks;

namespace LearnerApp.Common.TaskController
{
    public class SingleTaskExecutionRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTaskExecutionRunner"/> class.
        /// </summary>
        /// <param name="delayTimeSpan">The delay time before the execution inside actually ran.</param>
        public SingleTaskExecutionRunner(TimeSpan delayTimeSpan)
        {
            DelayTimeSpan = delayTimeSpan;
        }

        public delegate Task SingleTaskExecuteDelegate(CancellationToken cancellation);

        public TimeSpan DelayTimeSpan { get; }

        public CancellationTokenSource CurrentExecutingToken { get; set; }

        public void CancelCurrentTask()
        {
            CurrentExecutingToken?.Cancel();
        }

        public async Task RunTaskAsync(bool shouldConsumeTaskCanceled, params SingleTaskExecuteDelegate[] actions)
        {
            try
            {
                await InnerRunTaskAsync(actions);
            }
            catch (TaskCanceledException)
            {
                if (shouldConsumeTaskCanceled == false)
                {
                    throw;
                }
            }
        }

        private Task InnerRunTaskAsync(SingleTaskExecuteDelegate[] actions)
        {
            TimeSpan delayTimeSpan = DelayTimeSpan;
            if (CurrentExecutingToken != null)
            {
                CurrentExecutingToken?.Cancel();
            }
            else
            {
                // Execute the task immediately without delay
                delayTimeSpan = new TimeSpan(0);
            }

            CurrentExecutingToken = new CancellationTokenSource();

            // The first task is a trick that will decide if this one should be run or not
            Task firstTask = new Task(() =>
            {
                try
                {
                    Task.Delay(
                        (int)delayTimeSpan.TotalMilliseconds,
                        CurrentExecutingToken.Token).Wait();
                }
                catch (TaskCanceledException)
                {
                }
            });

            Task lastTask = firstTask;
            foreach (SingleTaskExecuteDelegate action in actions)
            {
                lastTask = lastTask.ContinueWith(
                    (Task task) =>
                    {
                        try
                        {
                            Task actionTask = action(CurrentExecutingToken.Token);
                            actionTask.Wait();
                        }
                        catch (TaskCanceledException)
                        {
                        }
                    },
                    CurrentExecutingToken.Token);
            }

            firstTask.Start();
            lastTask.ContinueWith(
                task =>
                {
                    CurrentExecutingToken = null;
                },
                CurrentExecutingToken.Token);
            return lastTask;
        }
    }
}
