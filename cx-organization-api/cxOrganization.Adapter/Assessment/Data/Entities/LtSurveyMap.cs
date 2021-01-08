using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class LT_SurveyMap.
    /// </summary>
    public class LtSurveyMap : IEntityTypeConfiguration<LtSurveyEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtSurveyMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtSurveyEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageID, t.SurveyID});

            // Properties
            builder.Property(t => t.LanguageID).ValueGeneratedNever();

            builder.Property(t => t.SurveyID).ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.Info)
                .IsRequired();

            builder.Property(t => t.FinishText)
                .IsRequired();

            builder.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("LT_Survey", "at");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.SurveyID).HasColumnName("SurveyID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.Info).HasColumnName("Info");
            builder.Property(t => t.FinishText).HasColumnName("FinishText");
            builder.Property(t => t.DisplayName).HasColumnName("DisplayName");

            // Relationships
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_Survey)
            //    .HasForeignKey(d => d.LanguageID);

            builder.HasOne(t => t.Survey)
                .WithMany(t => t.LtSurvey)
                .HasForeignKey(d => d.SurveyID);
        }
    }
}