using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class ResultMap.
    /// </summary>
    public class ResultMap : IEntityTypeConfiguration<ResultEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<ResultEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ResultID);

            // Properties
            builder.Property(t => t.ResultKey)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.chk)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            // Table & Column Mappings
            builder.ToTable("Result", "dbo");
            builder.Property(t => t.ResultID).HasColumnName("ResultID");
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.RoleID).HasColumnName("RoleID");
            builder.Property(t => t.UserID).HasColumnName("UserID");
            builder.Property(t => t.UserGroupID).HasColumnName("UserGroupID");
            builder.Property(t => t.SurveyID).HasColumnName("SurveyID");
            builder.Property(t => t.BatchID).HasColumnName("BatchID");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.PageNo).HasColumnName("PageNo");
            builder.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            builder.Property(t => t.Anonymous).HasColumnName("Anonymous");
            builder.Property(t => t.ShowBack).HasColumnName("ShowBack");
            builder.Property(t => t.ResultKey).HasColumnName("ResultKey");
            builder.Property(t => t.Email).HasColumnName("Email");
            builder.Property(t => t.chk).HasColumnName("chk");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.StatusTypeID).HasColumnName("StatusTypeID");
            builder.Property(t => t.validFrom).HasColumnName("validFrom");
            builder.Property(t => t.ValidTo).HasColumnName("ValidTo");
            builder.Property(t => t.DueDate).HasColumnName("DueDate");
            builder.Property(t => t.ParentResultId).HasColumnName("ParentResultID");
            builder.Property(t => t.ParentResultSurveyId).HasColumnName("ParentResultSurveyID");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
        }
    }
}

