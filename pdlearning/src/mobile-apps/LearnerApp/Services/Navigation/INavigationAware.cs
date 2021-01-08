using System.Threading.Tasks;

namespace LearnerApp.Services.Navigation
{
    /// <summary>
    /// Provides a way for objects involved in navigation to be notified of navigation activities.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationParameters">The navigation parameters.</param>
        /// <returns>A task.</returns>
        Task OnNavigatedTo(NavigationParameters navigationParameters);
    }
}
