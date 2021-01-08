using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineTeachingLevelConfiguration : BaseEntityTypeConfiguration<SearchEngineTeachingLevel>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineTeachingLevel> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.TeachingLevelId });

            builder.ToTable("SearchEngine_TeachingLevel", "staging");

            builder.Property(e => e.TeachingLevelId).HasColumnName("TeachingLevelID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineTeachingLevels)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
