using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using LearnerApp.Services.Identity;
using Xamarin.Forms;

namespace LearnerApp.Common
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private readonly IIdentityService _identityService;

        public AuthenticatedHttpClientHandler()
        {
            _identityService = DependencyService.Resolve<IIdentityService>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Follow: https://github.com/reactiveui/refit#authorization-dynamic-headers-redux.
            // See if the request has an authorize header
            var accountProperties = await _identityService.GetAccountPropertiesAsync();
            if (accountProperties != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accountProperties.AccessToken);

                // TODO: refactor logic add header.
                if (request.RequestUri.AbsolutePath.EndsWith("profile/upload"))
                {
                    request.Headers.TryAddWithoutValidation("Content-type", "multipart/form-data");
                }
                else
                {
                    request.Headers.TryAddWithoutValidation("Content-type", "application/json; charset=utf-8");
                }

                request.Headers.TryAddWithoutValidation("Pragma", "no-cache");
                request.Headers.TryAddWithoutValidation("Access-Control-Expose-Headers", "Correlation-Id");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.TryAddWithoutValidation("cxToken", "3001:2052");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
