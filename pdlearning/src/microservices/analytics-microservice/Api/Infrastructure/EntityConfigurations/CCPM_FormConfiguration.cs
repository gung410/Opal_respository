using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormConfiguration : BaseEntityTypeConfiguration<CCPM_Form>
    {
        public override void Configure(EntityTypeBuilder<CCPM_Form> builder)
        {
            builder.HasKey(e => e.FormId);

            builder.ToTable("ccpm_Form", "staging");

            builder.Property(e => e.FormId).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.OwnerDepartmentId).HasMaxLength(64);

            builder.Property(e => e.SqRatingType).HasMaxLength(50);

            builder.Property(e => e.Status).HasMaxLength(30);

            builder.Property(e => e.SurveyType).HasMaxLength(30);

            builder.Property(e => e.Title).HasMaxLength(1000);

            builder.Property(e => e.Type).HasMaxLength(30);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);
        }
    }
}
