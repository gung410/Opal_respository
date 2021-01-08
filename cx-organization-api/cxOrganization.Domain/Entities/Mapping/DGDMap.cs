using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class DGDMap : IEntityTypeConfiguration<DGDEntity>
    {
        public void Configure(EntityTypeBuilder<DGDEntity> builder)
        {
            builder.HasKey(t => new { t.DepartmentGroupId, t.DepartmentId });
            builder.ToTable("DG_D", "org");
            builder.Property(t => t.DepartmentGroupId).HasColumnName("DepartmentGroupID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");

            builder.HasOne(t => t.Department)
                    .WithMany(t => t.DG_Ds)
                    .HasForeignKey(t => t.DepartmentId);
            builder.HasOne(t => t.DepartmentGroup)
                    .WithMany(t => t.DG_Ds)
                    .HasForeignKey(t => t.DepartmentGroupId);
        }
    }
}
