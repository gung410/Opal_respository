using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineLearningAreaConfiguration : BaseEntityTypeConfiguration<SearchEngineLearningArea>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineLearningArea> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.LearningAreaId });

            builder.ToTable("SearchEngine_LearningArea", "staging");

            builder.Property(e => e.LearningAreaId).HasColumnName("LearningAreaID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineLearningAreas)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
