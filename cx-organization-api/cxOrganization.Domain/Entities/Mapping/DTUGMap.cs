using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class DTUGMap : IEntityTypeConfiguration<DTUGEntity>
    {
        public void Configure(EntityTypeBuilder<DTUGEntity> builder)
        {
            builder.HasKey(t => new { t.DepartmentTypeId, t.UserGroupId });
            builder.ToTable("DT_UG", "org");
            builder.Property(t => t.DepartmentTypeId).HasColumnName("DepartmentTypeID");
            builder.Property(t => t.UserGroupId).HasColumnName("UserGroupID");

            builder.HasOne(t => t.UserGroup)
                    .WithMany(t => t.DT_UGs)
                    .HasForeignKey(t => t.UserGroupId);
            builder.HasOne(t => t.DepartmentType)
                    .WithMany(t => t.DT_UGs)
                    .HasForeignKey(t => t.DepartmentTypeId);
        }
    }
}
