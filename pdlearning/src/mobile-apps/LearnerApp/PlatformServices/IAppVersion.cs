namespace LearnerApp.PlatformServices
{
    public interface IAppVersion
    {
        string GetVersion();

        string GetBuildNumber();
    }
}
