using System;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Form.Infrastructure
{
    public class FormDbContextResolver : BaseDbContextResolver
    {
        public FormDbContextResolver(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
