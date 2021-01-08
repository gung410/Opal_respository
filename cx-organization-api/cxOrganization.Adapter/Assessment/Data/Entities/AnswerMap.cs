using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class AnswerMap.
    /// </summary>
	public class AnswerMap : IEntityTypeConfiguration<AnswerEntity>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerMap"/> class.
        /// </summary>
		public void Configure(EntityTypeBuilder<AnswerEntity> builder)
        {
			// Primary Key
			builder.HasKey(t => t.AnswerID);

			// Properties
			builder.Property(t => t.Free)
				.IsRequired();
				
			// Table & Column Mappings
			builder.ToTable("Answer");
			builder.Property(t => t.AnswerID).HasColumnName("AnswerID");
            builder.Property(t => t.ItemId).HasColumnName("ItemId");
			builder.Property(t => t.ResultID).HasColumnName("ResultID");
			builder.Property(t => t.QuestionID).HasColumnName("QuestionID");
			builder.Property(t => t.AlternativeID).HasColumnName("AlternativeID");
			builder.Property(t => t.Value).HasColumnName("Value");
			builder.Property(t => t.DateValue).HasColumnName("DateValue");
			builder.Property(t => t.Free).HasColumnName("Free");
			builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.CreatedBy).HasColumnName("CreatedBy");

            // Relationships
            builder.HasOne(t => t.Result)
                .WithMany(t => t.Answers)
                .HasForeignKey(d => d.ResultID);
        }
	}
}

