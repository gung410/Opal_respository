using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Conexus.Opal.InboxPattern.HostedServices
{
    public abstract class InboxMessageHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        public InboxMessageHostedService(IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() => ApplicationStarted = true);
            lifetime.ApplicationStopping.Register(() => ApplicationStarted = false);
        }

        /// <summary>
        /// To determine that the application has started or not.
        /// </summary>
        protected bool ApplicationStarted { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Process, null, TimeSpan.Zero, TimerPeriodConfig());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// To config the period of the timer to trigger the <see cref="Process"/> method.
        /// </summary>
        /// <returns>The configuration as <see cref="TimeSpan"/> type.</returns>
        protected virtual TimeSpan TimerPeriodConfig()
        {
            return TimeSpan.FromMilliseconds(1000);
        }

        /// <summary>
        /// The main action of the hosted service that being triggered by the Timer.
        /// </summary>
        /// <param name="state">An object containing information to be used by the callback method, or null.</param>
        protected virtual void Process(object state)
        {
            // Do nothing.
        }
    }
}
