using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_LearningModeConfiguration : BaseEntityTypeConfiguration<MT_LearningMode>
    {
        public override void Configure(EntityTypeBuilder<MT_LearningMode> builder)
        {
            builder.ToTable("mt_LearningMode", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);
        }
    }
}
