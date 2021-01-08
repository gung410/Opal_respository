using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_DepartmentDepartmentTypeConfiguration : BaseEntityTypeConfiguration<SAM_DepartmentDepartmentType>
    {
        public override void Configure(EntityTypeBuilder<SAM_DepartmentDepartmentType> builder)
        {
            builder.HasKey(e => new { e.DepartmentId, e.DepartmentTypeId });

            builder.ToTable("sam_Department_DepartmentType", "staging");

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.DepartmentTypeId).HasMaxLength(64);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.SamDepartmentDepartmentType)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.DepartmentType)
                .WithMany(p => p.SamDepartmentDepartmentType)
                .HasForeignKey(d => d.DepartmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
