using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtDepartmentGroupMap.
    /// </summary>
    public class LtDepartmentGroupMap : IEntityTypeConfiguration<LtDepartmentGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtDepartmentGroupMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtDepartmentGroup> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageId, t.DepartmentGroupId});

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_DepartmentGroup", "org");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.DepartmentGroupId).HasColumnName("DepartmentGroupID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_DepartmentGroup)
            //    .HasForeignKey(d => d.LanguageId);

            builder.HasOne(t => t.DepartmentGroup)
                .WithMany(t => t.LT_DepartmentGroup)
                .HasForeignKey(d => d.DepartmentGroupId);
        }
    }
}