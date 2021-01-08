using System;
using System.Threading;
using System.Threading.Tasks;

namespace LearnerApp.Common.Helper
{
    /// <summary>
    /// This class is used for handling the case that it's triggered by multiple times for the same action like:
    /// (double, triple clicks), scrolling to load more...
    /// </summary>
    public class StressActionHandler
    {
        private readonly TimeSpan _delay;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public StressActionHandler()
        {
            _delay = TimeSpan.FromMilliseconds(1000);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StressActionHandler"/> class with custom delay.
        /// </summary>
        /// <param name="delay">The delay you want between each actions.</param>
        public StressActionHandler(TimeSpan delay)
        {
            _delay = delay;
        }

        /// <summary>
        /// Run void action.
        /// </summary>
        /// <param name="uiAction">synchronous action you need to run.</param>
        /// <returns>Return asynchronous action. This action will be completed after the delay time you defined in the constructor.</returns>
        public async Task Run(Action uiAction)
        {
            DateTime beforeWaiting = DateTime.Now;

            await _semaphore.WaitAsync();

            double totalTimeWaitingInMilis = (DateTime.Now - beforeWaiting).TotalMilliseconds;

            if (totalTimeWaitingInMilis > 10)
            {
                _semaphore.Release();
                return;
            }

            try
            {
                uiAction?.Invoke();
                await Task.Delay(_delay);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Run asynchronous action.
        /// </summary>
        /// <param name="uiAction">Asynchronous action you need to run.</param>
        /// <returns>Return asynchronous action. This action is completed after waiting the input asynchronous action to be completed and the delay time.</returns>
        public async Task RunAsync(Func<Task> uiAction)
        {
            DateTime beforeWaiting = DateTime.Now;

            await _semaphore.WaitAsync();

            double totalTimeWaitingInMilis = (DateTime.Now - beforeWaiting).TotalMilliseconds;

            if (totalTimeWaitingInMilis > 10)
            {
                _semaphore.Release();
                return;
            }

            try
            {
                if (uiAction != null)
                {
                    await uiAction.Invoke();
                }

                await Task.Delay(_delay);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
