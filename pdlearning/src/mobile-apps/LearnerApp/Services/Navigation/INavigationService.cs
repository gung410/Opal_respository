using System.Threading.Tasks;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Services.Navigation
{
    public interface INavigationService
    {
        BaseViewModel PreviousPageViewModel { get; }

        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : IRoutingAware;

        Task NavigateToAsync<TViewModel>(NavigationParameters parameters) where TViewModel : IRoutingAware;

        Task NavigateToAsync(string routeName, NavigationParameters parameter);

        Task GoBack();

        Task NavigateToMainTabItem(string route, NavigationParameters parameters);
    }
}
