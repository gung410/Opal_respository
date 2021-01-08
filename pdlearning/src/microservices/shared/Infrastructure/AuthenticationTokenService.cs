using System;
using System.Net.Http;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Constants;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Exceptions;

namespace Conexus.Opal.Microservice.Infrastructure
{
    public class AuthenticationTokenService : IAuthenticationTokenService
    {
        private readonly string _authorityUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public AuthenticationTokenService(IConfiguration configuration)
        {
            _authorityUrl = configuration[ConfigurationKeys.AuthorityUrl];
            _clientId = configuration[ConfigurationKeys.ClientId];
            _clientSecret = configuration[ConfigurationKeys.ClientSecret];
        }

        private TokenResponse Token { get; set; }

        private DateTime ExpiryTime { get; set; }

        public Task<TokenResponse> GetToken() => Task.Run(async () =>
        {
            if (Token != null && ExpiryTime > DateTime.UtcNow)
            {
                return Token;
            }

            using (var client = new HttpClient())
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = $"{_authorityUrl}/connect/token",
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                });

                if (tokenResponse.IsError)
                {
                    throw new GeneralException($"There is an error when getting token from the endpoint {_authorityUrl}. Details: {tokenResponse.ErrorDescription}");
                }

                Token = tokenResponse;
                ExpiryTime = DateTime.UtcNow.AddSeconds(Token.ExpiresIn);
                return Token;
            }
        });
    }
}
