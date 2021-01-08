using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserBookmarksCourseConfiguration : BaseEntityTypeConfiguration<Learner_UserBookmarksCourse>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserBookmarksCourse> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("UserBookmarkCourseId")
                .ValueGeneratedOnAdd();

            builder.ToTable("learner_UserBookmarksCourse", "staging");

            builder.Property(e => e.ChangedDate).HasColumnName("UpdateDate");

            builder.Property(e => e.Comment).HasMaxLength(2000);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.Property(e => e.ItemType)
               .IsRequired()
               .HasMaxLength(30);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserBookmarksCourse)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
