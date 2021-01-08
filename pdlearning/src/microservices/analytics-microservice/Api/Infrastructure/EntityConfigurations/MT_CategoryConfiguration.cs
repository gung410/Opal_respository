using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_CategoryConfiguration : BaseEntityTypeConfiguration<MT_Category>
    {
        public override void Configure(EntityTypeBuilder<MT_Category> builder)
        {
            builder.HasKey(e => e.CategoryId);

            builder.ToTable("mt_Category", "staging");

            builder.Property(e => e.CategoryId)
                .HasColumnName("CategoryID")
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
