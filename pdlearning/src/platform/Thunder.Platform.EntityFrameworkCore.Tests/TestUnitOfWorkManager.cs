using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Xunit;

namespace Thunder.Platform.EntityFrameworkCore.Tests
{
    public class TestUnitOfWorkManager : BaseEntityFrameworkCoreTest
    {
        [Fact]
        public void CreateUnitOfWorkShouldOk()
        {
            var uowManager = Provider.GetService<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                uow.Complete();
            }
        }

        [Fact]
        public void CreateInnerUnitOfWorkShouldOk()
        {
            var uowManager = Provider.GetService<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                using (var innerUow = uowManager.Begin())
                {
                    innerUow.Complete();
                }

                uow.Complete();
            }
        }
    }
}
