using System.Text;
using Foundation;
using LearnerApp.Common;
using LearnerApp.Helpers;
using LearnerApp.iOS.Services;
using LearnerApp.PlatformServices;
using Xamarin.Forms;
using static PCLCrypto.WinRTCrypto;
using HashAlgorithm = PCLCrypto.HashAlgorithm;

[assembly: Dependency(typeof(OAuth2PkceSupport))]

namespace LearnerApp.iOS.Services
{
    public class OAuth2PkceSupport : IOAuth2PkceSupport
    {
        public CodeChallengeResult CreateCodeChallenge()
        {
            var codeVerifier = RandomNumberGenerator.CreateUniqueId();
            var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(codeVerifier)));
            CryptographicBuffer.CopyToByteArray(challengeBuffer, out var challengeBytes);

            return new CodeChallengeResult(codeVerifier, Base64Url.Encode(challengeBytes));
        }

        /// <inheritdoc />
        public void ManualClearAllCookies()
        {
            NSHttpCookieStorage cookieStorage = NSHttpCookieStorage.SharedStorage;

            if (cookieStorage.Cookies.Length != 0)
            {
                foreach (var cookie in cookieStorage.Cookies)
                {
                    cookieStorage.DeleteCookie(cookie);
                }
            }
        }
    }
}
