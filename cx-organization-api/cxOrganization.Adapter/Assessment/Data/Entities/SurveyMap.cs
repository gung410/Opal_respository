using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class SurveyMap.
    /// </summary>
    public class SurveyMap : IEntityTypeConfiguration<SurveyEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<SurveyEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SurveyID);

            // Properties
            builder.Property(t => t.LinkURL)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(t => t.FinishURL)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(t => t.ReportDB)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.ReportServer)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.StyleSheet)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.OLAPServer)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.OLAPDB)
                .IsRequired()
                .HasMaxLength(64);

            // Table & Column Mappings
            builder.ToTable("Survey", "at");
            builder.Property(t => t.SurveyID).HasColumnName("SurveyID");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.HierarchyID).HasColumnName("HierarchyID");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.Anonymous).HasColumnName("Anonymous");
            builder.Property(t => t.Status).HasColumnName("Status");
            builder.Property(t => t.ShowBack).HasColumnName("ShowBack");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.ButtonPlacement).HasColumnName("ButtonPlacement");
            builder.Property(t => t.UsePageNo).HasColumnName("UsePageNo");
            builder.Property(t => t.LinkURL).HasColumnName("LinkURL");
            builder.Property(t => t.FinishURL).HasColumnName("FinishURL");
            builder.Property(t => t.CreateResult).HasColumnName("CreateResult");
            //builder.Property(t => t.DefaultMinResults).HasColumnName("DefaultMinResults");
            builder.Property(t => t.ReportDB).HasColumnName("ReportDB");
            builder.Property(t => t.ReportServer).HasColumnName("ReportServer");
            builder.Property(t => t.StyleSheet).HasColumnName("StyleSheet");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.LastProcessed).HasColumnName("LastProcessed");
            builder.Property(t => t.ReProcessOLAP).HasColumnName("ReProcessOLAP");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.OLAPServer).HasColumnName("OLAPServer");
            builder.Property(t => t.OLAPDB).HasColumnName("OLAPDB");
            builder.Property(t => t.PeriodID).HasColumnName("PeriodID");
            builder.Property(t => t.ProcessCategorys).HasColumnName("ProcessCategorys");
            builder.Property(t => t.DeleteResultOnUserDelete).HasColumnName("DeleteResultOnUserDelete");
            builder.Property(t => t.LastProcessedResultID).HasColumnName("LastProcessedResultID");
            builder.Property(t => t.LastProcessedAnswerID).HasColumnName("LastProcessedAnswerID");
            builder.Property(t => t.ExtId).HasColumnName("ExtId");
            builder.Property(t => t.Tag).HasColumnName("Tag");

            // Relationships
            builder.HasOne(t => t.Activity)
                .WithMany(t => t.Surveys)
                .HasForeignKey(d => d.ActivityID);

            builder.HasOne(t => t.Period)
                .WithMany(t => t.Surveys)
                .HasForeignKey(d => d.PeriodID);

            //HasOptional(t => t.Hierarchy)
            //    .WithMany(t => t.Surveys)
            //    .HasForeignKey(d => d.HierarchyID);

            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.Surveys)
            //    .HasForeignKey(d => d.LanguageID);
        }
    }
}