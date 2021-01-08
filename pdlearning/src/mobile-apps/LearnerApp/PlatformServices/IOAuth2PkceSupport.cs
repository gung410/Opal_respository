namespace LearnerApp.PlatformServices
{
    public interface IOAuth2PkceSupport
    {
        CodeChallengeResult CreateCodeChallenge();

        void ManualClearAllCookies();
    }

    public class CodeChallengeResult
    {
        public CodeChallengeResult(string codeVerifier, string codeChallenge)
        {
            CodeVerifier = codeVerifier;
            CodeChallenge = codeChallenge;
        }

        public string CodeVerifier { get; }

        public string CodeChallenge { get; }
    }
}
