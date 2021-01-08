using System;
using Xunit;

namespace LearnerApp.UnitTest
{
    public class XamarinFixture : IDisposable
    {
        public XamarinFixture()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        public void Dispose()
        {
        }
    }
}
