using System;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Backend.ApiHandler;
using LearnerApp.Services.DataManager.InitLogin;
using LearnerApp.Services.Identity;
using LearnerApp.ViewModels;
using LearnerApp.ViewModels.Base;
using LearnerApp.Views;
using Xamarin.Forms;

namespace LearnerApp.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IIdentityService _identityService;

        public NavigationService()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
        }

        public BaseViewModel PreviousPageViewModel
        {
            get
            {
                if (Application.Current.MainPage is CustomNavigationView mainPage)
                {
                    var viewModel = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext;
                    return viewModel as BaseViewModel;
                }

                return null;
            }
        }

        public async Task InitializeAsync()
        {
            var isAuthenticated = await _identityService.IsAuthenticated();

            if (isAuthenticated)
            {
                await _identityService.SetupCloudFrontCookieForImageService();

                // We need navigate to Blank Page to get metadata from API
                // We just call metadata once time initialize App
                // After navigate to Blank Page, page will navigate to Nav Controller Page
                var initLoginDataManager = new InitLoginDataManager();
                await initLoginDataManager.InitLoginData();

                await MainTopbarViewModel.Instance.OnNavigatedTo(null);
                await NavigateToMainTabItem(NavigationRoutes.Home, null);
            }
            else
            {
                await NavigateToAsync<LoginViewModel>();
            }
        }

        public async Task NavigateToMainTabItem(string route, NavigationParameters parameters)
        {
            await Navigate(NavigationRoutes.GetNavigationRoutesForTabItem(route), parameters);
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : IRoutingAware
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(NavigationParameters parameters) where TViewModel : IRoutingAware
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameters);
        }

        public async Task NavigateToAsync(string routeName, NavigationParameters parameter)
        {
            await Navigate(routeName, parameter);
        }

        public async Task GoBack()
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private async Task GetAccountProperties()
        {
            var apiHandler = new ApiHandler();

            var accountProperties = await apiHandler.ExecuteBackendService(_identityService.GetAccountPropertiesAsync);

            if (accountProperties.HasEmptyResult())
            {
                return;
            }

            Application.Current.Properties.AddAccountProperties(accountProperties.Payload);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, NavigationParameters parameters)
        {
            var viewModel = Activator.CreateInstance(viewModelType);
            if (viewModel is IRoutingAware routingAware)
            {
                await Navigate(
                    routingAware.RoutingName,
                    parameters);
            }

            if (viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private async Task Navigate(string finalRoute, NavigationParameters parameters)
        {
            if (parameters != null)
            {
                NavigationParameterManager.SetTransferParameter(finalRoute, parameters);
            }

            await Shell.Current.GoToAsync(
                finalRoute,
                true);
        }
    }
}
