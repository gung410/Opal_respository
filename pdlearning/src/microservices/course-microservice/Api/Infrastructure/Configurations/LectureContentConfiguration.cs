using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class LectureContentConfiguration : BaseConfiguration<LectureContent>
    {
        public override void Configure(EntityTypeBuilder<LectureContent> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

            builder.Property(e => e.MimeType).HasMaxLength(EntitiesConstants.LectureMimeTypeLength);

            builder.Property(e => e.Title)
                .HasMaxLength(EntitiesConstants.LectureContentTitleLength);

            builder.Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<LectureContentType>())
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(e => e.ExternalID)
              .HasMaxLength(EntitiesConstants.ExternalIDLength)
              .IsRequired(false);

            builder.OwnsOne(p => p.QuizConfig);

            builder.OwnsOne(p => p.DigitalContentConfig);

            builder.HasIndex(p => new { p.LectureId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
        }
    }
}
