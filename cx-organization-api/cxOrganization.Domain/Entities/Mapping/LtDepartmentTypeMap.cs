using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtDepartmentTypeMap.
    /// </summary>
    public class LtDepartmentTypeMap : IEntityTypeConfiguration<LtDepartmentTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtDepartmentTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtDepartmentTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageId, t.DepartmentTypeId});

            // Properties

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_DepartmentType", "org");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.DepartmentTypeId).HasColumnName("DepartmentTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_DepartmentType)
            //    .HasForeignKey(d => d.LanguageId);

            builder.HasOne(t => t.DepartmentType)
                .WithMany(t => t.LT_DepartmentType)
                .HasForeignKey(d => d.DepartmentTypeId);
        }
    }
}