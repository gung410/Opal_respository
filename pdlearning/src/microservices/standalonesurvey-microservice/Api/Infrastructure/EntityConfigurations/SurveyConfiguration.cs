using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveyConfiguration : BaseEntityConfiguration<Domain.Entities.StandaloneSurvey>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.StandaloneSurvey> builder)
        {
            builder.Property(p => p.Title).HasMaxLength(Domain.Entities.StandaloneSurvey.MaxTitleLength);
            builder
                .Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<SurveyStatus>())
                .HasColumnType("varchar(30)");
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder
              .Property(e => e.SqRatingType)
              .HasConversion(new EnumToStringConverter<SqRatingType>())
              .HasColumnType("varchar(50)");

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
