using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class FormParticipantConfiguration : BaseEntityTypeConfiguration<FormParticipant>
    {
        public override void Configure(EntityTypeBuilder<FormParticipant> builder)
        {
            builder.Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<FormParticipantStatus>())
                .HasDefaultValue(FormParticipantStatus.NotStarted)
                .HasMaxLength(30)
                .IsUnicode(false);
        }
    }
}
