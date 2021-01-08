using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtActivityMap : IEntityTypeConfiguration<LtActivityEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LT_ActivityMap" /> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtActivityEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageID, t.ActivityID });

            // Properties
            builder.Property(t => t.LanguageID)
                .ValueGeneratedNever();

            builder.Property(t => t.ActivityID)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Info)
                .IsRequired();

            builder.Property(t => t.StartText)
                .IsRequired();

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.SurveyName)
                .IsRequired();

            builder.Property(t => t.RoleName)
                .IsRequired();

            builder.Property(t => t.BatchName)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.ShortName)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("LT_Activity", "at");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Info).HasColumnName("Info");
            builder.Property(t => t.StartText).HasColumnName("StartText");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.SurveyName).HasColumnName("SurveyName");
            builder.Property(t => t.RoleName).HasColumnName("RoleName");
            builder.Property(t => t.BatchName).HasColumnName("BatchName");
            builder.Property(t => t.DisplayName).HasColumnName("DisplayName");
            builder.Property(t => t.ShortName).HasColumnName("ShortName");

            // Relationships
            builder.HasOne(t => t.Activity)
                .WithMany(t => t.LtActivities)
                .HasForeignKey(d => d.ActivityID);
        }
    }
}
