using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Uploader.Infrastructure
{
    public class UploaderDbContextResolver : BaseDbContextResolver
    {
        public UploaderDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
