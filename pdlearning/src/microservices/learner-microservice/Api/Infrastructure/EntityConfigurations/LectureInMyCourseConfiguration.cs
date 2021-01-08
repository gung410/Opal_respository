using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class LectureInMyCourseConfiguration : BaseEntityTypeConfiguration<LectureInMyCourse>
    {
        public override void Configure(EntityTypeBuilder<LectureInMyCourse> builder)
        {
            builder.Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<LectureStatus>())
                .HasColumnType("varchar(30)");

            builder.Property(e => e.ReviewStatus)
                .HasMaxLength(LectureInMyCourse.MaxReviewStatusLength);

            builder.Property(e => e.Version)
                .HasColumnType("varchar(5)");

            builder.HasIndex(p => new { p.MyCourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.LectureId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
        }
    }
}
