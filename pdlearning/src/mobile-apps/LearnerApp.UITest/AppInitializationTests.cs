using NUnit.Framework;
using Xamarin.UITest;

namespace LearnerApp.UITest
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class AppInitializationTests
    {
        private readonly Platform _platform;
        private IApp _app;

        public AppInitializationTests(Platform platform)
        {
            this._platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);
        }

        [Test]
        public void App_startup_without_error()
        {
            _app.WaitForElement("BtnLogin");
            _app.Screenshot("Login screen.");
        }
    }
}
