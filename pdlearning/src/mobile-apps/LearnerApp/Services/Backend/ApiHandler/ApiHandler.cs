using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.PlatformServices;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.ExceptionHandler;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using LearnerApp.ViewModels;
using Newtonsoft.Json;
using Refit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.Services.Backend.ApiHandler
{
    public class ApiHandler
    {
        private SemaphoreSlim _checkSingleSessionSemaphore = new SemaphoreSlim(1);

        public ApiHandler()
        {
            IdentityService = DependencyService.Resolve<IIdentityService>();
            DialogService = DependencyService.Resolve<IDialogService>();
            NavigationService = DependencyService.Resolve<INavigationService>();
            ExceptionHandler = DependencyService.Resolve<IExceptionHandler>();
        }

        protected IIdentityService IdentityService { get; }

        protected IDialogService DialogService { get; }

        protected INavigationService NavigationService { get; }

        protected IExceptionHandler ExceptionHandler { get; }

         /// <summary>
        /// Execute api call inside a try/catch block to handle common HTTP Status Code.
        /// The reason why we do this because there is no concept of Error middleware or general error handler in refit library.
        /// </summary>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <param name="action">The calling action.</param>
        /// <returns>The result of api call.</returns>
        public async Task<Models.ApiResponse<TPayload>> ExecuteBackendService<TPayload>(Func<Task<TPayload>> action)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DialogService.ShowAlertAsync("No internet connection", "Try again");

                return null;
            }

            try
            {
                return new Models.ApiResponse<TPayload>
                {
                    Payload = await action.Invoke()
                };
            }
            catch (ApiException exception)
            {
                // Do not wait for execution
                Device.BeginInvokeOnMainThread(async () => await HandleApiException(exception));

                return new Models.ApiResponse<TPayload>
                {
                    Error = GetErrorMessage(exception)
                };
            }
        }

        public async Task<bool> ExecuteBackendService(Func<Task> action)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DialogService.ShowAlertAsync("No internet connection", "Try again");
                return false;
            }

            try
            {
                await action.Invoke();
                return true;
            }
            catch (ApiException exception)
            {
                // Do not wait for execution
                Device.BeginInvokeOnMainThread(async () => await HandleApiException(exception));
                return false;
            }
        }

        private async Task HandleApiException(ApiException exception)
        {
            if (IsSingleSessionError(exception))
            {
                if (_checkSingleSessionSemaphore.CurrentCount == 0)
                {
                    return;
                }

                await _checkSingleSessionSemaphore.WaitAsync();
                try
                {
                    // Set IsRedirectLoginPage = true prevent duplicate error message.
                    ErrorState.IsRedirectLoginPage = true;

                    // Set IsLoginPageLoaded = false to raise login view need to reload.
                    LoginState.IsLoginPageLoaded = false;

                    // We clean cookies and account properties through session timeout, another device logged to disable pop-up clean session in the web view when login again.
                    var pkceSupport = DependencyService.Resolve<IOAuth2PkceSupport>();
                    pkceSupport.ManualClearAllCookies();
                    IdentityService.RemoveAccountProperties();
                    IdentityService.RemoveCloudFrontCookieInfo();

                    var tcs = new TaskCompletionSource<object>();
                    await this.DialogService.ShowAlertAsync(GetErrorMessage(exception), cancelTextBtn: "OK", onClosed: async (confirmed) =>
                    {
                        if (!confirmed)
                        {
                            return;
                        }

                        tcs.SetResult(null);
                        await this.NavigationService.NavigateToAsync<LoginViewModel>();
                    });
                    await tcs.Task;
                    await Task.Delay(5000); // Wait at least 5 sec for in case another API fail because of this.
                }
                finally
                {
                    _checkSingleSessionSemaphore.Release();
                }
            }
            else
            {
                ExceptionHandler.HandleException(exception);

                // Error message display in not case: session timeout and another device login.
                if (!ErrorState.IsRedirectLoginPage)
                {
                    await DialogService.ShowAlertAsync(GetErrorMessage(exception));
                }
            }
        }

        private string GetErrorMessage(ApiException exception)
        {
            string error;
            if (IsSingleSessionError(exception))
            {
                error = exception.StatusCode == HttpStatusCode.Forbidden ? "You're logged on another device." : "Please re-login to renew your session.";
            }
            else
            {
                if (!ErrorState.IsRedirectLoginPage)
                {
                    error = GenerateErrorMessageFor(exception);
                }
                else
                {
                    error = string.Empty;
                }
            }

            return error;
        }

        private bool IsSingleSessionError(ApiException exception)
        {
            if (exception.StatusCode == HttpStatusCode.Forbidden || exception.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (exception.Content.Contains("Key not authorised")
                    || exception.Content.Contains("Key not authorized")
                    || exception.Content.Contains("Forbidden")
                    || exception.Content.Contains("Unauthorized request"))
                {
                    return true;
                }
            }

            return false;
        }

        private string GenerateErrorMessageFor(ApiException exception)
        {
            try
            {
                var errorHandler = JsonConvert.DeserializeObject<ErrorHandler>(exception.Content);

                if (!string.IsNullOrEmpty(errorHandler.GetErrorContent()))
                {
                    return errorHandler.GetErrorContent();
                }
            }
            catch
            {
                return GenerateErrorMessageByStatusCode(exception);
            }

            return GenerateErrorMessageByStatusCode(exception);
        }

        private string GenerateErrorMessageByStatusCode(ApiException exception)
        {
            return exception.StatusCode switch
            {
                HttpStatusCode.InternalServerError =>
                "Sorry! An error has occurred. Please come back later when we have fixed the problem. Thank you.",

                HttpStatusCode.NotFound =>
                "Sorry, it looks like the destination you are trying to reach is not accessible.",

                HttpStatusCode.BadRequest => "Your request is invalid or improperly formed.",

                _ => "An error has occurred. Please contact the Helpdesk if the issue persists."
            };
        }
    }
}
