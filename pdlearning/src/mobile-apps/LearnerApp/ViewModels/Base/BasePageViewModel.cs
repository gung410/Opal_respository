using System;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Base
{
    /// <summary>
    /// Base view model for pages that having title at the top and requires caching mode.
    /// </summary>
    public abstract class BasePageViewModel : BaseViewModel, INavigationAware, IRoutingAware, IDisposable
    {
        private DateTime _authenticatedCheckTime;
        private bool _alreadyNavigated = false;

        protected BasePageViewModel()
        {
            IdentityService = DependencyService.Resolve<IIdentityService>();
        }

        public CancellationTokenSource CurrentCancellationTokenSource { get; protected set; } = new CancellationTokenSource();

        public abstract string PageTitle { get; }

        public abstract string RoutingName { get; }

        public PageCachingMode CachingMode { get; set; } = PageCachingMode.OnlyFirstNavigation;

        protected IIdentityService IdentityService { get; }

        public Task OnNavigatedTo(NavigationParameters navigationParameters)
        {
            if (!_alreadyNavigated || HasForceNavigationFlag(navigationParameters))
            {
                _alreadyNavigated = true;
                _authenticatedCheckTime = IdentityService.AuthenticatedCheckTime;
                return InternalNavigatedTo(navigationParameters);
            }

            // If the caching mode is OnlyFirstNavigation, there is no need to reload data again.
            // This flag is an option for screen that may require data loading all the time to override if needed.
            if (CachingMode == PageCachingMode.OnlyFirstNavigation)
            {
                return Task.CompletedTask;
            }

            return InternalNavigatedTo(navigationParameters);
        }

        /// <summary>
        /// This will be triggered when the view is loaded again.
        /// </summary>
        /// <returns>Use asynchronous function to handle some API calls or long waiting operations.</returns>
        public virtual Task OnComeBack()
        {
            return Task.CompletedTask;
        }

        protected virtual Task InternalNavigatedTo(NavigationParameters navigationParameters)
        {
            return Task.CompletedTask;
        }

        private bool HasForceNavigationFlag(NavigationParameters navigationParameters)
        {
            if (navigationParameters != null)
            {
                bool forceNavigation = navigationParameters.GetParameter<bool>("ForceNavigation");
                return forceNavigation;
            }

            return false;
        }
    }
}
