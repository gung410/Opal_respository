using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_PeriodMap.
    /// </summary>
    public class LtPeriodMap : IEntityTypeConfiguration<LtPeriodEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtPeriodMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtPeriodEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageId, t.PeriodId});

            // Properties

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_Period", "at");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.PeriodId).HasColumnName("PeriodID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            builder.HasOne(t => t.Period)
                .WithMany(t => t.LtPeriods)
                .HasForeignKey(d => d.PeriodId);
        }
    }
}