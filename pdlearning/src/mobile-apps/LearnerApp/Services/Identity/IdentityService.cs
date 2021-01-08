using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Config;
using LearnerApp.Common;
using LearnerApp.Models;
using LearnerApp.PlatformServices;
using LearnerApp.Services.ExceptionHandler;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LearnerApp.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private const string AccountPropertiesKey = "ConexusSts";
        private const string CloudFrontCookieInfo = "ConexusCloudFrontCookieInfo";
        private const string AuthenticationUrlFormat = "{0}?client_id={1}&redirect_uri={2}&scope={3}&state={4}&response_type=code&returnUrl={5}&response_mode=query&nonce={6}&code_challenge={7}&code_challenge_method=S256";

        private string _codeVerifier;

        public IdentityService()
        {
        }

        public static string SessionId { get; set; }

        public static Dictionary<string, bool> AccessRights { get; set; }

        public DateTime AuthenticatedCheckTime { get; private set; }

        public async Task<IdentityModel> Authenticate(IOAuth2PkceSupport pkceSupport)
        {
            try
            {
                var codeChallengeResult = pkceSupport.CreateCodeChallenge();
                _codeVerifier = codeChallengeResult.CodeVerifier;

                var authenticationUrl = string.Format(
                    AuthenticationUrlFormat,
                    $"{GlobalSettings.BackendServiceIdm}{GlobalSettings.AuthAuthorizeUrl}",
                    GlobalSettings.AuthClientId,
                    GlobalSettings.AuthCallbackUrl,
                    GlobalSettings.AuthScope,
                    Guid.NewGuid().ToString("N"),
                    GlobalSettings.AuthCallbackUrl,
                    Guid.NewGuid().ToString("N"),
                    codeChallengeResult.CodeChallenge);

                var authResult = await WebAuthenticator
                    .AuthenticateAsync(
                        url: new Uri(new Uri(authenticationUrl)
                            .AbsoluteUri), // Authentication Url contains white spaces, special chars, etc.
                        callbackUrl: new Uri(GlobalSettings.AuthCallbackUrl));

                var code = authResult.Properties["code"];

                var queryValues = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", GlobalSettings.AuthCallbackUrl },
                    { "client_id", GlobalSettings.AuthClientId }
                };

                if (!string.IsNullOrEmpty(this._codeVerifier))
                {
                    queryValues.Add("code_verifier", this._codeVerifier);
                }

                var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{GlobalSettings.BackendServiceIdm}{GlobalSettings.AuthAccessTokenUrl}")
                {
                    Content = new FormUrlEncodedContent(queryValues)
                };
                var response = await httpClient.SendAsync(request);
                var encodedString = await response.Content.ReadAsStringAsync();

                // Expect JSON response:
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(encodedString);
                if (dictionary.ContainsKey("error"))
                {
                    throw new Exception($"Error authenticating: {dictionary["error"]}");
                }

                if (!dictionary.ContainsKey("access_token"))
                {
                    throw new Exception("Expected access_token in access token response, but did not receive one.");
                }

                var expiresInSeconds = int.Parse(dictionary["expires_in"]);
                var userInfo = TokenAnalyzer.Analyze(dictionary["id_token"]);
                var tokenExpiryTime = DateTime.Now.AddSeconds(expiresInSeconds);

                var identity = new IdentityModel(
                    accessToken: dictionary["access_token"],
                    idToken: dictionary["id_token"],
                    user: userInfo,
                    tokenExpiryTime: tokenExpiryTime);

                await SetAccountPropertiesAsync(JsonConvert.SerializeObject(identity));

                return identity;
            }
            catch (Exception e)
            {
                DependencyService.Resolve<IExceptionHandler>().HandleException(e);

                return IdentityModel.Default();
            }
        }

        public async Task<IdentityModel> GetAccountPropertiesAsync()
        {
            var account = await SecureStorage.GetAsync(AccountPropertiesKey);

            if (string.IsNullOrEmpty(account))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<IdentityModel>(account);
        }

        public async Task SetAccountPropertiesAsync(string props)
        {
            // Sign out and sign in with a different account
            AuthenticatedCheckTime = DateTime.Now;

            await SecureStorage.SetAsync(AccountPropertiesKey, props);
        }

        public void RemoveAccountProperties()
        {
            SecureStorage.Remove(AccountPropertiesKey);
        }

        /// <inheritdoc />
        public async Task StoreCloudFrontCookieInfo(CloudFrontCookieInfo cookieInfo)
        {
            var cookieService = DependencyService.Resolve<ICloudFrontCookieSetup>();
            await cookieService.SetupCloudFrontCookie(CreateCookieCollection(cookieInfo));
            await SecureStorage.SetAsync(CloudFrontCookieInfo, JsonConvert.SerializeObject(cookieInfo));
        }

        /// <inheritdoc />
        public void RemoveCloudFrontCookieInfo()
        {
            SecureStorage.Remove(CloudFrontCookieInfo);
        }

        /// <inheritdoc />
        public async Task SetupCloudFrontCookieForImageService()
        {
            var cookieInfoString = await SecureStorage.GetAsync(CloudFrontCookieInfo);
            if (string.IsNullOrEmpty(cookieInfoString))
            {
                return;
            }

            var cookieInfo = JsonConvert.DeserializeObject<CloudFrontCookieInfo>(cookieInfoString);
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(CreateCookieCollection(cookieInfo));
            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    CookieContainer = cookieContainer,
                    UseCookies = true
                })
            });
        }

        /// <inheritdoc />
        public async Task<bool> IsAuthenticated()
        {
            // Verify authentication when you reopen the application
            AuthenticatedCheckTime = DateTime.Now;

            var accountProperties = await GetAccountPropertiesAsync();

            // In case of user not logged in.
            if (accountProperties == null)
            {
                return false;
            }

            if (accountProperties.TokenExpiryTime < DateTime.Now)
            {
                return false;
            }

            if (string.IsNullOrEmpty(accountProperties.OnBoarded) ||
                OnBoardingState.NotYetOnBoarded.Equals(accountProperties.OnBoarded))
            {
                return false;
            }

            return true;
        }

        private CookieCollection CreateCookieCollection(CloudFrontCookieInfo cookieInfo)
        {
            var cloudFrontUri = new Uri(GlobalSettings.CloudFrontUrl);
            Cookie CreateCookie(string name, string value)
            {
                return new Cookie(name, value, "/", cloudFrontUri.Host) { Secure = true, HttpOnly = true };
            }

            var cookieCollection = new CookieCollection
            {
                CreateCookie(cookieInfo.KeyPairIdKey, cookieInfo.KeyPairIdValue),
                CreateCookie(cookieInfo.PolicyKey, cookieInfo.PolicyValue),
                CreateCookie(cookieInfo.SignatureKey, cookieInfo.SignatureValue)
            };

            return cookieCollection;
        }
    }
}
