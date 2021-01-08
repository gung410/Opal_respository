using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineServiceSchemeConfiguration : BaseEntityTypeConfiguration<SearchEngineServiceScheme>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineServiceScheme> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.ServiceSchemeId });

            builder.ToTable("SearchEngine_ServiceScheme", "staging");

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineServiceSchemes)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
