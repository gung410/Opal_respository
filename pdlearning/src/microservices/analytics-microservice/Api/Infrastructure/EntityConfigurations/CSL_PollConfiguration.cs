using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_PollConfiguration : BaseEntityTypeConfiguration<CSL_Poll>
    {
        public override void Configure(EntityTypeBuilder<CSL_Poll> builder)
        {
            builder.ToTable("csl_Poll", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Question).IsRequired();

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
