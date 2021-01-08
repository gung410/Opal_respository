using System;
using LearnerApp.Models;
using LearnerApp.Services.Dialog;
using LearnerApp.Services.ExceptionHandler;
using LearnerApp.Services.Identity;
using LearnerApp.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xamarin.Forms.Internals;

namespace LearnerApp.UnitTest.Mock
{
    public class MockServiceManager
    {
        private readonly ServiceCollection _servicesCollection = new ServiceCollection();

        private ServiceProvider _servicesProvider;

        public MockServiceManager()
        {
            DependencyResolver.ResolveUsing(Resolver);
        }

        public static MockServiceManager Current { get; } = new MockServiceManager();

        public MockServiceManager MockIdentityService()
        {
            var identityService = new Moq.Mock<IIdentityService>();
            identityService.Setup(x => x.GetAccountPropertiesAsync())
                .ReturnsAsync(new IdentityModel("abc", "abc", new UserInfo(), DateTime.Now));

            identityService.Setup(x => x.AuthenticatedCheckTime).Returns(DateTime.Now);

            return MockService(identityService.Object);
        }

        public MockServiceManager MockDialogService()
        {
            var dialogService = new Moq.Mock<IDialogService>();

            return MockService(dialogService.Object);
        }

        public MockServiceManager MockNavigationService()
        {
            var dialogService = new Moq.Mock<INavigationService>();

            return MockService(dialogService.Object);
        }

        public MockServiceManager MockExceptionHandler()
        {
            var dialogService = new Moq.Mock<IExceptionHandler>();

            return MockService(dialogService.Object);
        }

        public MockServiceManager MockService<TService>(TService service) where TService : class
        {
            _servicesCollection.AddSingleton(service);
            return this;
        }

        public void Build()
        {
            _servicesProvider = _servicesCollection.BuildServiceProvider();
        }

        private object Resolver(Type type, object[] args)
        {
            if (_servicesProvider == null)
            {
                throw new Exception("You have to call Build first in order to resolve dependency");
            }

            return _servicesProvider.GetService(type);
        }
    }
}
