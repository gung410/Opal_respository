using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class A_SMap.
    /// </summary>
    public class ActivityStatusTypeMap : IEntityTypeConfiguration<ActivityStatusTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityStatusTypeMap" /> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<ActivityStatusTypeEntity> builder)
        {
            // Primary Key
            //builder.HasKey(t => new {t.ActivityID, t.StatusTypeID});
            builder.HasKey(t => new { t.ASID });

            // Properties
            builder.Property(t => t.ActivityID)
                .ValueGeneratedNever();

            builder.Property(t => t.StatusTypeID)
                .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("A_S", "at");
            builder.Property(t => t.ASID).HasColumnName("ASID");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.StatusTypeID).HasColumnName("StatusTypeID");
            builder.Property(t => t.No).HasColumnName("No");
        }
    }
}