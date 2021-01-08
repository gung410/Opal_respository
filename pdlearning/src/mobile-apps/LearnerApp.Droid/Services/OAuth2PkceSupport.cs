using System.Text;
using Android.Webkit;
using LearnerApp.Common;
using LearnerApp.Droid.Services;
using LearnerApp.Helpers;
using LearnerApp.PlatformServices;
using Xamarin.Forms;
using static PCLCrypto.WinRTCrypto;
using HashAlgorithm = PCLCrypto.HashAlgorithm;

[assembly: Dependency(typeof(OAuth2PkceSupport))]

namespace LearnerApp.Droid.Services
{
    /// <inheritdoc/>
    public class OAuth2PkceSupport : IOAuth2PkceSupport
    {
        /// <inheritdoc/>
        public CodeChallengeResult CreateCodeChallenge()
        {
            var codeVerifier = RandomNumberGenerator.CreateUniqueId();
            var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(codeVerifier)));
            CryptographicBuffer.CopyToByteArray(challengeBuffer, out var challengeBytes);

            return new CodeChallengeResult(codeVerifier, Base64Url.Encode(challengeBytes));
        }

        /// <inheritdoc/>
        public void ManualClearAllCookies()
        {
            if (CookieManager.Instance.HasCookies)
            {
                CookieManager.Instance.RemoveAllCookies(null);
                CookieManager.Instance.RemoveSessionCookie();
                CookieManager.Instance.Flush();
            }
        }
    }
}
