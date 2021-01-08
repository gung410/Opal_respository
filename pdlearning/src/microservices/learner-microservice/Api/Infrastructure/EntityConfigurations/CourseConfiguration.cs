using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class CourseConfiguration : BaseEntityTypeConfiguration<Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(e => e.Version)
                   .HasMaxLength(10)
                    .IsRequired(false)
                   .IsUnicode(false);

            builder.Property(e => e.PDActivityType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.LearningMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.CourseCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.CourseName)
                    .IsRequired(false)
                    .HasMaxLength(2000);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<CourseStatus>())
               .HasDefaultValue(CourseStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.ContentStatus)
               .HasConversion(new EnumToStringConverter<ContentStatus>())
               .HasDefaultValue(ContentStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.CourseType)
               .HasConversion(new EnumToStringConverter<CourseType>())
               .HasMaxLength(15)
               .IsUnicode(false)
               .HasDefaultValue(CourseType.New);

            builder.Property(e => e.Source)
                .IsUnicode()
                .HasMaxLength(255);
        }
    }
}
