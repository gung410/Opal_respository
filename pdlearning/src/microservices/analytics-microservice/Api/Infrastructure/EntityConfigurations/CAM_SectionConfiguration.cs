using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_SectionConfiguration : BaseEntityTypeConfiguration<CAM_Section>
    {
        public override void Configure(EntityTypeBuilder<CAM_Section> builder)
        {
            builder.HasKey(e => e.SectionId);

            builder.ToTable("cam_Section", "staging");

            builder.Property(e => e.SectionId)
                .HasColumnName("SectionID")
                .ValueGeneratedNever();

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamSection)
                .HasForeignKey(d => d.ClassRunId);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamSection)
                .HasForeignKey(d => d.CourseId);
        }
    }
}
