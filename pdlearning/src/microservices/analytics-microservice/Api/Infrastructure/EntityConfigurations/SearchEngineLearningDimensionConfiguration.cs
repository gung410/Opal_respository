using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineLearningDimensionConfiguration : BaseEntityTypeConfiguration<SearchEngineLearningDimension>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineLearningDimension> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.LearningDimensionId });

            builder.ToTable("SearchEngine_LearningDimension", "staging");

            builder.Property(e => e.LearningDimensionId).HasColumnName("LearningDimensionID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineLearningDimensions)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
