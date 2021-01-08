using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class UDUTMap : IEntityTypeConfiguration<UDUTEntity>
    {
        public void Configure(EntityTypeBuilder<UDUTEntity> builder)
        {
            builder.HasKey(t => new { t.U_DId, t.UsertypeId });
            builder.ToTable("U_D_UT", "org");
            builder.Property(t => t.U_DId).HasColumnName("U_DID");
            builder.Property(t => t.UsertypeId).HasColumnName("UsertypeID");

            builder.HasOne(t => t.UserType)
                    .WithMany(t => t.U_D_UTs)
                    .HasForeignKey(t => t.UsertypeId);
            builder.HasOne(t => t.UserDepartment)
                    .WithMany(t => t.U_D_UTs)
                    .HasForeignKey(t => t.U_DId);
        }
    }
}
