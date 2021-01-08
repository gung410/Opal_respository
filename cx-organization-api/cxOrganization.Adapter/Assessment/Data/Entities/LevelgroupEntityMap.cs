using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LevelGroupEntityMap : IEntityTypeConfiguration<LevelGroupEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelGroupMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LevelGroupEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LevelGroupId);

            // Properties
            builder.Property(t => t.Tag)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            builder.ToTable("LevelGroup", "at");
            builder.Property(t => t.LevelGroupId).HasColumnName("LevelGroupID");
            builder.Property(t => t.ActivityId).HasColumnName("ActivityID");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
            builder.Property(t => t.DepartmentId).HasColumnName("Departmentid");
            builder.Property(t => t.RoleId).HasColumnName("Roleid");
            builder.Property(t => t.Created).HasColumnName("Created");

            // Relationships
            builder.HasOne(t => t.Activity)
                .WithMany(t => t.LevelGroups)
                .HasForeignKey(d => d.ActivityId);
        }
    }
}
