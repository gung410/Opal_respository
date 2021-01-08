using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_NatureOfCourseConfiguration : BaseEntityTypeConfiguration<MT_NatureOfCourse>
    {
        public override void Configure(EntityTypeBuilder<MT_NatureOfCourse> builder)
        {
            builder.HasKey(e => e.NatureOfCourseId);

            builder.ToTable("mt_NatureOfCourse", "staging");

            builder.Property(e => e.NatureOfCourseId).ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);
        }
    }
}
