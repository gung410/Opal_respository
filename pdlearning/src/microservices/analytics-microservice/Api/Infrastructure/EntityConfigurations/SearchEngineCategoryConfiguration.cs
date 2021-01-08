using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineCategoryConfiguration : BaseEntityTypeConfiguration<SearchEngineCategory>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineCategory> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.CategoryId });

            builder.ToTable("SearchEngine_Category", "staging");

            builder.Property(e => e.CategoryId).HasColumnName("CategoryID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineCategories)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
