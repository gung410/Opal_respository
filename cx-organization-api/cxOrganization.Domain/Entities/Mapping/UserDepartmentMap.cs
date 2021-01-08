using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class U_DMap.
    /// </summary>
    public class UserDepartmentMap : IEntityTypeConfiguration<UserDepartmentEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDepartmentMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<UserDepartmentEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.U_DId);

            // Properties
            // Table & Column Mappings
            builder.ToTable("U_D", "org");
            builder.Property(t => t.U_DId).HasColumnName("U_DID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.Selected).HasColumnName("Selected");
            builder.Property(t => t.Created).HasColumnName("Created");

            // Relationships
            builder.HasOne(t => t.User)
                .WithMany(t => t.U_D)
                .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}