using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_DigitalContentConfiguration : BaseEntityTypeConfiguration<CCPM_DigitalContent>
    {
        public override void Configure(EntityTypeBuilder<CCPM_DigitalContent> builder)
        {
            builder.HasKey(e => e.DigitalContentId);

            builder.ToTable("ccpm_DigitalContent", "staging");

            builder.Property(e => e.DigitalContentId).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Discriminator).IsRequired();

            builder.Property(e => e.FileExtension)
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(e => e.FileName).HasMaxLength(255);

            builder.Property(e => e.OwnerDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(28)
                .IsUnicode(false);

            builder.Property(e => e.Title).HasMaxLength(255);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(25)
                .IsUnicode(false);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
