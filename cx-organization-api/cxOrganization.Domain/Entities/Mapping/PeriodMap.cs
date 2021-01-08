using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PeriodMap.
    /// </summary>
    public class PeriodMap : IEntityTypeConfiguration<PeriodEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<PeriodEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PeriodId);

            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("Period", "at");
            builder.Property(t => t.PeriodId).HasColumnName("PeriodID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Startdate).HasColumnName("Startdate");
            builder.Property(t => t.Enddate).HasColumnName("Enddate");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
        }
    }
}