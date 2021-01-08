using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_StatusTypeMap.
    /// </summary>
    public class LtStatusTypeMap : IEntityTypeConfiguration<LtStatusTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtStatusTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtStatusTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageID, t.StatusTypeID});

            // Properties
            builder.Property(t => t.LanguageID)
                .ValueGeneratedNever();

            builder.Property(t => t.StatusTypeID)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.DefaultActionName)
                .HasMaxLength(250);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(t => t.DefaultActionDescription)
                .HasMaxLength(250);

            // Table & Column Mappings
            builder.ToTable("LT_StatusType", "at");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.StatusTypeID).HasColumnName("StatusTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.DefaultActionName).HasColumnName("DefaultActionName");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.DefaultActionDescription).HasColumnName("DefaultActionDescription");

            // Relationships
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_StatusType)
            //    .HasForeignKey(d => d.LanguageID);

            builder.HasOne(t => t.StatusType)
                .WithMany(t => t.LtStatusType)
                .HasForeignKey(d => d.StatusTypeID);
        }
    }
}