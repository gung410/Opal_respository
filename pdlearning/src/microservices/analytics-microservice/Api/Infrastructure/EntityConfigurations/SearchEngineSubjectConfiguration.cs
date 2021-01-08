using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SearchEngineSubjectConfiguration : BaseEntityTypeConfiguration<SearchEngineSubject>
    {
        public override void Configure(EntityTypeBuilder<SearchEngineSubject> builder)
        {
            builder.HasKey(e => new { e.SearchEngineId, e.SubjectId });

            builder.ToTable("SearchEngine_Subject", "staging");

            builder.Property(e => e.SubjectId).HasColumnName("SubjectID");

            builder.HasOne(d => d.SearchEngine)
                .WithMany(p => p.SearchEngineSubjects)
                .HasForeignKey(d => d.SearchEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
