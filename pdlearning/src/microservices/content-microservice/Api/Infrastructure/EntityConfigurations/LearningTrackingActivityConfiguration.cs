using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class LearningTrackingActivityConfiguration : BaseEntityTypeConfiguration<LearningTracking>
    {
        public override void Configure(EntityTypeBuilder<LearningTracking> builder)
        {
            builder.Property(e => e.TrackingAction)
                .ConfigureForEnum();

            builder.Property(e => e.TrackingType)
                .ConfigureForEnum();
        }
    }
}
