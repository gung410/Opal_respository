using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_WikiPageConfiguration : BaseEntityTypeConfiguration<CSL_WikiPage>
    {
        public override void Configure(EntityTypeBuilder<CSL_WikiPage> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("csl_WikiPage", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
