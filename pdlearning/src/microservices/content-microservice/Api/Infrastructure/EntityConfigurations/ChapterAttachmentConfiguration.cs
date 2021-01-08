using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class ChapterAttachmentConfiguration : BaseEntityTypeConfiguration<ChapterAttachment>
    {
        public override void Configure(EntityTypeBuilder<ChapterAttachment> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasOne(d => d.Chapter)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ObjectId);
        }
    }
}
