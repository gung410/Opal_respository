using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_TrackConfiguartion : BaseEntityTypeConfiguration<MT_Track>
    {
        public override void Configure(EntityTypeBuilder<MT_Track> builder)
        {
            builder.HasKey(e => e.TrackId);

            builder.ToTable("mt_Track", "staging");

            builder.Property(e => e.TrackId)
                .HasColumnName("TrackID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.Type).HasMaxLength(100);
        }
    }
}
