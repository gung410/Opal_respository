using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class AnnouncementTemplateConfiguration : BaseConfiguration<AnnouncementTemplate>
    {
        public override void Configure(EntityTypeBuilder<AnnouncementTemplate> builder)
        {
            base.Configure(builder);
            builder.ToTable("AnnouncementTemplate");

            // Technical Columns
            builder.Property(e => e.FullTextSearch)
                .IsRequired(false);

            builder.Property(e => e.FullTextSearchKey)
                    .IsRequired(true)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValue(string.Empty);

            // Column Indexes
            builder.HasIndex(p => new { p.Title, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.CreatedDate);
            builder.HasIndex(p => p.FullTextSearchKey).IsUnique();
        }
    }
}
