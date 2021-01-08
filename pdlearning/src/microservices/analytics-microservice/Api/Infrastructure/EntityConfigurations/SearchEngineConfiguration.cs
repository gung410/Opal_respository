using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineConfiguration : BaseEntityTypeConfiguration<SearchEngine>
    {
        public override void Configure(EntityTypeBuilder<SearchEngine> builder)
        {
            builder.ToTable("SearchEngine", "staging");

            builder.Property(e => e.Id).HasColumnName("SearchEngineId").ValueGeneratedOnAdd();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.PdactivityTypeId).HasColumnName("PDActivityTypeId");

            builder.Property(e => e.SearchText).IsRequired();
        }
    }
}
