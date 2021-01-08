using System;
using Microsoft.Extensions.DependencyInjection;

namespace Thunder.Platform.EntityFrameworkCore.Tests.Dummy
{
    public class TestDbContextResolver : BaseDbContextResolver
    {
        public TestDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
