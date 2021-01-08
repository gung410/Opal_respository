using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_GroupConfiguration : BaseEntityTypeConfiguration<CSL_Group>
    {
        public override void Configure(EntityTypeBuilder<CSL_Group> builder)
        {
            builder.ToTable("csl_Group", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Name).HasMaxLength(45);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
