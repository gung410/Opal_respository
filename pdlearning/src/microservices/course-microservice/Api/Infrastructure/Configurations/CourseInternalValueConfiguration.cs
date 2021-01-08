using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CourseInternalValueConfiguration : BaseEntityTypeConfiguration<CourseInternalValue>
    {
        public override void Configure(EntityTypeBuilder<CourseInternalValue> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Value).HasMaxLength(100);
            builder.HasOne(p => p.Course)
                .WithMany(p => p.CourseInternalValues)
                .HasForeignKey(p => p.CourseId);
            builder.Property(p => p.Type)
                   .HasConversion(new EnumToStringConverter<CourseInternalValueType>())
                   .HasDefaultValue(CourseInternalValueType.CourseFacilitatorIds)
                   .HasMaxLength(50)
                   .IsUnicode(false);
            builder.HasIndex(p => p.CourseId);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Value);
            builder.HasIndex(p => new { p.Value, p.Type, p.CourseId });
            builder.HasIndex(p => new { p.CourseId, p.Value, p.Type });
        }
    }
}
