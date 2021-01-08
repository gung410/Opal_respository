using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class BlockoutDateConfiguration : BaseConfiguration<BlockoutDate>
    {
        public override void Configure(EntityTypeBuilder<BlockoutDate> builder)
        {
            base.Configure(builder);
            builder.ToTable("BlockoutDate");

            builder.Property(e => e.ServiceSchemes)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.FullTextSearchKey)
                    .IsRequired(true)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValue(string.Empty);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<BlockoutDateStatus>())
               .HasDefaultValue(BlockoutDateStatus.Draft)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.IsConfirmed)
                .HasDefaultValue(false);

            builder.HasIndex(p => new { p.UpdatedBy, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.StartDay, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.StartMonth, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.EndDay, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.EndMonth, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ValidYear, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.PlanningCycleId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsConfirmed, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.StartDate, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.EndDate, p.IsDeleted, p.FullTextSearchKey });

            builder.HasIndex(p => new { p.ValidYear, p.StartMonth, p.StartDay, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ValidYear, p.EndMonth, p.EndDay, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ValidYear, p.StartMonth, p.StartDay, p.EndMonth, p.EndDay, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.ChangedDate });

            // Technical Columns Indexes
            builder.HasIndex(p => p.FullTextSearchKey).IsUnique();

            builder.Property(e => e.ServiceSchemesFullTextSearch)
                .IsRequired(false);
        }
    }
}
