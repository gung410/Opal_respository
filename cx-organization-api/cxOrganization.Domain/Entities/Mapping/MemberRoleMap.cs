using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace cxOrganization.Domain.Entities
{
    public class MemberRoleMap : IEntityTypeConfiguration<MemberRoleEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<MemberRoleEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.MemberRoleId);
            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("MemberRole", "org");
            builder.Property(t => t.MemberRoleId).HasColumnName("MemberRoleID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
            builder.Property(t => t.CodeName).HasColumnName("CodeName");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
        }
    }
}
