using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineDevelopmentRoleConfiguration : BaseEntityTypeConfiguration<SearchEngineDevelopmentRole>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineDevelopmentRole> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.DevelopmentRoleId });

            builder.ToTable("SearchEngine_DevelopmentRole", "staging");

            builder.Property(e => e.DevelopmentRoleId).HasColumnName("DevelopmentRoleID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineDevelopmentRoles)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
