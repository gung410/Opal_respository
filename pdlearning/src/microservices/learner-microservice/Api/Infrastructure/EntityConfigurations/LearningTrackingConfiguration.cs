using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class LearningTrackingConfiguration : BaseEntityTypeConfiguration<LearningTracking>
    {
        public override void Configure(EntityTypeBuilder<LearningTracking> builder)
        {
            builder.Property(e => e.TrackingAction)
                .HasConversion(new EnumToStringConverter<LearningTrackingAction>())
                .HasDefaultValue(LearningTrackingAction.View)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.TrackingType)
                .HasConversion(new EnumToStringConverter<LearningTrackingType>())
                .HasDefaultValue(LearningTrackingType.DigitalContent)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
