using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class DigitalContentConfiguration : BaseEntityTypeConfiguration<DigitalContent>
    {
        public override void Configure(EntityTypeBuilder<DigitalContent> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(e => e.Title)
                .HasMaxLength(255);

            builder.Property(e => e.Type)
                .ConfigureForEnum();

            builder.Property(e => e.Status)
                .ConfigureForEnum();

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.Property(e => e.ExternalId)
                .IsUnicode(false)
                .HasMaxLength(255);

            builder.Property(e => e.RepositoryName)
                .IsUnicode()
                .HasMaxLength(100);

            builder.Property(e => e.Source)
                .IsUnicode()
                .HasMaxLength(255);

            builder.Property(e => e.Ownership)
                .ConfigureForEnum();

            builder.Property(e => e.LicenseType)
                .ConfigureForEnum();

            builder.Property(e => e.TermsOfUse)
                .IsUnicode()
                .HasMaxLength(4000);

            builder.Property(e => e.LicenseTerritory)
                .ConfigureForEnum();

            builder.Property(e => e.Publisher)
                .IsUnicode()
                .HasMaxLength(100);

            builder.Property(e => e.Copyright)
                .IsUnicode()
                .HasMaxLength(100);

            builder.Property(e => e.IsAllowReusable)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsAllowDownload)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsAllowModification)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.AcknowledgementAndCredit)
                .IsUnicode()
                .HasMaxLength(4000);

            builder.Property(e => e.Remarks)
                .IsUnicode()
                .HasMaxLength(4000);

            builder.Property(e => e.AverageRating)
                .IsUnicode(false);

            builder.Property(e => e.ReviewCount)
                .IsUnicode(false);
        }
    }
}
