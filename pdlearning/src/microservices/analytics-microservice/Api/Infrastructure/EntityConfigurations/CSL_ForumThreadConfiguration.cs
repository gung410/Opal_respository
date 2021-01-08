using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_ForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_ForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_ForumThread> builder)
        {
            builder.ToTable("csl_ForumThread", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
