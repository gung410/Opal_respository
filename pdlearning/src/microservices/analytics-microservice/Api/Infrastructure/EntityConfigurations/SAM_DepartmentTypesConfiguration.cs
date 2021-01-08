using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_DepartmentTypesConfiguration : BaseEntityTypeConfiguration<SAM_DepartmentTypes>
    {
        public override void Configure(EntityTypeBuilder<SAM_DepartmentTypes> builder)
        {
            builder.HasKey(e => e.DepartmentTypeId);

            builder.ToTable("sam_DepartmentTypes", "staging");

            builder.Property(e => e.DepartmentTypeId).HasMaxLength(64);

            builder.Property(e => e.ArchetypeId).HasMaxLength(64);

            builder.Property(e => e.Code).HasMaxLength(256);

            builder.Property(e => e.ExtDepartmentTypeId).HasColumnName("ExtDepartmentTypeID");

            builder.Property(e => e.ExtId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.MasterId).HasMaxLength(64);

            builder.Property(e => e.Name).HasMaxLength(64);

            builder.Property(e => e.ParentId).HasMaxLength(64);
        }
    }
}
