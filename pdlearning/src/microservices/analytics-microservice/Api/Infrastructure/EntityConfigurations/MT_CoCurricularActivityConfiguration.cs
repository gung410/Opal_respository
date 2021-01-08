using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_CoCurricularActivityConfiguration : BaseEntityTypeConfiguration<MT_CoCurricularActivity>
    {
        public override void Configure(EntityTypeBuilder<MT_CoCurricularActivity> builder)
        {
            builder.HasKey(e => e.CoCurricularActivityId);

            builder.ToTable("mt_CoCurricularActivity", "staging");

            builder.Property(e => e.CoCurricularActivityId)
                .HasColumnName("CoCurricularActivityID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(512);

            builder.Property(e => e.Type).HasMaxLength(512);
        }
    }
}
