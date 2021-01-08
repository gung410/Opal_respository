using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveyParticipantConfiguration : BaseEntityConfiguration<SurveyParticipant>
    {
        public override void Configure(EntityTypeBuilder<SurveyParticipant> builder)
        {
            builder.Property(e => e.Status)
              .HasConversion(new EnumToStringConverter<SurveyParticipantStatus>())
              .HasDefaultValue(SurveyParticipantStatus.NotStarted)
              .HasMaxLength(30)
              .IsUnicode(false);
            builder.Property(p => p.IsStarted).HasDefaultValue(false);
            base.Configure(builder);
        }
    }
}
