using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_ServiceSchemeConfiguration : BaseEntityTypeConfiguration<MT_ServiceScheme>
    {
        public override void Configure(EntityTypeBuilder<MT_ServiceScheme> builder)
        {
            builder.HasKey(e => e.ServiceSchemeId);

            builder.ToTable("mt_ServiceScheme", "staging");

            builder.Property(e => e.ServiceSchemeId)
                .HasColumnName("ServiceSchemeID")
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
