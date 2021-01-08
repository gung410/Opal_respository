using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class FormParticipantConfiguration : BaseEntityConfiguration<FormParticipant>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormParticipant> builder)
        {
            builder.Property(e => e.Status)
              .HasConversion(new EnumToStringConverter<FormParticipantStatus>())
              .HasDefaultValue(FormParticipantStatus.NotStarted)
              .HasMaxLength(30)
              .IsUnicode(false);
            builder.Property(p => p.IsStarted).HasDefaultValue(false);
            base.Configure(builder);
        }
    }
}
