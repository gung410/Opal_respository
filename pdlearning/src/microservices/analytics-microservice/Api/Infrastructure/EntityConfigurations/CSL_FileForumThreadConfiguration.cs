using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_FileForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_FileForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_FileForumThread> builder)
        {
            builder.ToTable("csl_FileForumThread", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.Property(e => e.Guid)
                .HasMaxLength(45)
                .IsUnicode(false);

            builder.Property(e => e.MimeType)
                .HasMaxLength(150)
                .IsUnicode(false);

            builder.Property(e => e.Size)
                .HasMaxLength(45)
                .IsUnicode(false);

            builder.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
