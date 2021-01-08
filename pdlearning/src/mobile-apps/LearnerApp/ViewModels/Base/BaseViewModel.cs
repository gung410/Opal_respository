using System;
using System.Threading.Tasks;
using LearnerApp.Common;
using LearnerApp.Services.Backend.ApiHandler;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.ExceptionHandler;
using LearnerApp.Services.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using Xamarin.Forms;

namespace LearnerApp.ViewModels.Base
{
    public abstract class BaseViewModel : ExtendedBindableObject, IDisposable
    {
        private static readonly AuthenticatedHttpClientHandler AuthenticatedHttpClientHandler = new AuthenticatedHttpClientHandler();

        private bool _isBusy;

        protected BaseViewModel()
        {
            ApiHandler = new ApiHandler();
            DialogService = DependencyService.Resolve<IDialogService>();
            NavigationService = DependencyService.Resolve<INavigationService>();
            ExceptionHandler = DependencyService.Resolve<IExceptionHandler>();
        }

        ~BaseViewModel()
        {
            Dispose();
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        protected ApiHandler ApiHandler { get; }

        protected IDialogService DialogService { get; }

        protected INavigationService NavigationService { get; }

        protected IExceptionHandler ExceptionHandler { get; }

        public static T CreateRestClientFor<T>(string baseUrl)
        {
            return RestService.For<T>(
                baseUrl,
                new RefitSettings
                {
                    ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                            NullValueHandling = NullValueHandling.Ignore
                        }),
                    HttpMessageHandlerFactory = () => AuthenticatedHttpClientHandler
                });
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Execute api call inside a try/catch block to handle common HTTP Status Code.
        /// The reason why we do this because there is no concept of Error middleware or general error handler in refit library.
        /// </summary>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <param name="action">The calling action.</param>
        /// <returns>The result of api call.</returns>
        protected Task<Models.ApiResponse<TPayload>> ExecuteBackendService<TPayload>(Func<Task<TPayload>> action)
        {
            return ApiHandler.ExecuteBackendService(action);
        }

        protected Task<bool> ExecuteBackendService(Func<Task> action)
        {
            return ApiHandler.ExecuteBackendService(action);
        }
    }
}
