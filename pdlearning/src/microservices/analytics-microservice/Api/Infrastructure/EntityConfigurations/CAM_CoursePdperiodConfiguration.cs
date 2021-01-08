using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CoursePdperiodConfiguration : BaseEntityTypeConfiguration<CAM_CoursePdperiod>
    {
        public override void Configure(EntityTypeBuilder<CAM_CoursePdperiod> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.PdperiodId });

            builder.ToTable("cam_Course_PDPeriod", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.PdperiodId).HasColumnName("PDPeriodID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCoursePdperiod)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Pdperiod)
                .WithMany(p => p.CamCoursePdperiod)
                .HasForeignKey(d => d.PdperiodId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
