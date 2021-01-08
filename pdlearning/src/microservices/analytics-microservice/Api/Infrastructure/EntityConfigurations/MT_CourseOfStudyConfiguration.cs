using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_CourseOfStudyConfiguration : BaseEntityTypeConfiguration<MT_CourseOfStudy>
    {
        public override void Configure(EntityTypeBuilder<MT_CourseOfStudy> builder)
        {
            builder.HasKey(e => e.CourseOfStudyId);

            builder.ToTable("mt_CourseOfStudy", "staging");

            builder.Property(e => e.CourseOfStudyId)
                .HasColumnName("CourseOfStudyID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(512);

            builder.Property(e => e.Type).HasMaxLength(512);
        }
    }
}
