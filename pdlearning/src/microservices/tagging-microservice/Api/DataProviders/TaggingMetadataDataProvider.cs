using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Tagging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Conexus.Opal.Microservice.Tagging.DataProviders
{
    public class TaggingMetadataDataProvider : BaseMetadataDataProvider
    {
        public TaggingMetadataDataProvider(TaggingDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
        }
    }
}
