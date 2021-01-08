using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineLearningFrameworkConfiguration : BaseEntityTypeConfiguration<SearchEngineLearningFramework>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineLearningFramework> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.LearningFrameworkId });

            builder.ToTable("SearchEngine_LearningFramework", "staging");

            builder.Property(e => e.LearningFrameworkId).HasColumnName("LearningFrameworkID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineLearningFrameworks)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
