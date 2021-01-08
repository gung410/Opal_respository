using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveySectionConfiguration : BaseEntityConfiguration<SurveySection>
    {
        public override void Configure(EntityTypeBuilder<SurveySection> builder)
        {
            builder.Property(p => p.MainDescription).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.AdditionalDescription).HasColumnType("nvarchar(MAX)");

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { FormId = p.SurveyId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
