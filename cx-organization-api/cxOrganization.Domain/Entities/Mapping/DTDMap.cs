using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class DTDMap : IEntityTypeConfiguration<DTDEntity>
    {
        public void Configure(EntityTypeBuilder<DTDEntity> builder)
        {
            builder.HasKey(t => new { t.DepartmentTypeId, t.DepartmentId });
            builder.ToTable("DT_D", "org");
            builder.Property(t => t.DepartmentTypeId).HasColumnName("DepartmentTypeID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");

            builder.HasOne(t => t.Department)
                    .WithMany(t => t.DT_Ds)
                    .HasForeignKey(t => t.DepartmentId);
            builder.HasOne(t => t.DepartmentType)
                    .WithMany(t => t.DT_Ds)
                    .HasForeignKey(t => t.DepartmentTypeId);
        }
    }
}
