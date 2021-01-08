using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class DepartmentMap.
    /// </summary>
    public class DepartmentMap : IEntityTypeConfiguration<DepartmentEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.Adress)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(t => t.PostalCode)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(t => t.City)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.OrgNo)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Tag)
                .IsRequired();

            builder.Property(t => t.CountryCode);

            builder.Property(p => p.EntityVersion).IsConcurrencyToken();

            // Table & Column Mappings
            builder.ToTable("Department", "org");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.Adress).HasColumnName("Adress");
            builder.Property(t => t.PostalCode).HasColumnName("PostalCode");
            builder.Property(t => t.City).HasColumnName("City");
            builder.Property(t => t.OrgNo).HasColumnName("OrgNo");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.Locked).HasColumnName("Locked");
            builder.Property(t => t.CountryCode).HasColumnName("CountryCode");
            builder.Property(t => t.EntityVersion).HasColumnName("EntityVersion");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.LastSynchronized).HasColumnName("LastSynchronized");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
            builder.Property(t => t.DynamicAttributes).HasColumnName("DynamicAttributes");
        }
    }
}
