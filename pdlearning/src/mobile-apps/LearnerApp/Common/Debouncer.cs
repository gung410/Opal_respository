using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LearnerApp.Common
{
    /// <summary>
    /// This is acted like an rxjs DebouceTime to prevent user crazy click events during a specific amount of time.
    /// </summary>
    public class Debouncer
    {
        // Use a locking object to prevent the debouncer to trigger again while the func is still running
        private readonly object _lockThis = new object();
        private readonly int _millisecondsToWait;

        private List<CancellationTokenSource> _stepperCancelTokens = new List<CancellationTokenSource>();

        public Debouncer(int millisecondsToWait = 300)
        {
            this._millisecondsToWait = millisecondsToWait;
        }

        public void Debouce(Action func)
        {
            CancelAllStepperTokens(); // Cancel all api requests;
            var newTokenSrc = new CancellationTokenSource();
            lock (_lockThis)
            {
                _stepperCancelTokens.Add(newTokenSrc);
            }

            Task.Delay(_millisecondsToWait, newTokenSrc.Token)
                .ContinueWith(
                    task =>
                    {
                        // if it hasn't been cancelled
                        if (!newTokenSrc.IsCancellationRequested)
                        {
                            // Cancel any that remain (there shouldn't be any)
                            CancelAllStepperTokens();

                            // set to new list
                            _stepperCancelTokens = new List<CancellationTokenSource>();

                            lock (_lockThis)
                            {
                                // run
                                func();
                            }
                        }
                    });
        }

        private void CancelAllStepperTokens()
        {
            foreach (var token in _stepperCancelTokens.Where(token => !token.IsCancellationRequested))
            {
                token.Cancel();
            }
        }
    }
}
