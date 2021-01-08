using System.Net.Http;
using System.Threading.Tasks;
using Microservice.WebinarProxy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarProxy.Application.Services
{
    public class SessionValidator : ISessionValidator
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly AuthenticationOptions _authOption;
        private readonly ILogger<SessionValidator> _logger;

        public SessionValidator(
            IHttpClientFactory httpFactory,
            IOptions<AuthenticationOptions> authOptions,
            ILogger<SessionValidator> logger)
        {
            _httpFactory = httpFactory;
            _authOption = authOptions.Value;
            _logger = logger;
        }

        public async Task<bool> ValidateByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri(_authOption.CheckSessionEndpoint)
            };
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + token);

            var httpClient = _httpFactory.CreateClient();

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var responseMessage = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Validate session failed. Detail: {ResponseMessage}", responseMessage);

            return false;
        }
    }
}
