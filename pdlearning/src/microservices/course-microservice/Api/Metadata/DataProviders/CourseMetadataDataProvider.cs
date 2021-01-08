using Conexus.Opal.Microservice.Metadata.DataProviders;
using Microservice.Course.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Microservice.Course.Metadata.DataProviders
{
    public class CourseMetadataDataProvider : BaseMetadataDataProvider
    {
        public CourseMetadataDataProvider(CourseDbContext dbContext, ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
        }

        protected override string TagIdColumn => "Id";
    }
}
