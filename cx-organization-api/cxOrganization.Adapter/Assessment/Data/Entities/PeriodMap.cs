using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
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
            builder.HasKey(t => t.PeriodID);

            // Properties
            builder.Property(t => t.ExtID)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("Period", "at");
            builder.Property(t => t.PeriodID).HasColumnName("PeriodID");
            builder.Property(t => t.Ownerid).HasColumnName("Ownerid");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.StartDate).HasColumnName("Startdate");
            builder.Property(t => t.EndDate).HasColumnName("Enddate");
            builder.Property(t => t.ExtID).HasColumnName("ExtID");
        }
    }
}