using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class H_DMap.
    /// </summary>
    public class HierarchyDepartmentMap : IEntityTypeConfiguration<HierarchyDepartmentEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyDepartmentMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<HierarchyDepartmentEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.HDId);

            // Properties
            builder.Property(t => t.Path)
                .IsRequired()
                .HasMaxLength(900);

            builder.Property(t => t.PathName)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            builder.ToTable("H_D", "org");
            builder.Property(t => t.HDId).HasColumnName("HDID");
            builder.Property(t => t.HierarchyId).HasColumnName("HierarchyID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");
            builder.Property(t => t.ParentId).HasColumnName("ParentID");
            builder.Property(t => t.Path).HasColumnName("Path");
            builder.Property(t => t.PathName).HasColumnName("PathName");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");

            // Relationships
            builder.HasOne(t => t.Department)
               .WithMany(t => t.H_D)            
               .HasForeignKey(d => d.DepartmentId);


            builder.HasOne(t => t.Parent)                
                .WithMany(t=>t.H_Ds)
                .HasForeignKey(d => d.ParentId);

            builder.HasOne(t => t.Hierarchy)
                 .WithMany(t => t.H_Ds)
                 .HasForeignKey(d => d.HierarchyId);
        }
    }
}