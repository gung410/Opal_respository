using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineLearningSubAreaConfiguration : BaseEntityTypeConfiguration<SearchEngineLearningSubArea>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineLearningSubArea> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.LearningSubAreaId });

            builder.ToTable("SearchEngine_LearningSubArea", "staging");

            builder.Property(e => e.LearningSubAreaId).HasColumnName("LearningSubAreaID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineLearningSubAreas)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
