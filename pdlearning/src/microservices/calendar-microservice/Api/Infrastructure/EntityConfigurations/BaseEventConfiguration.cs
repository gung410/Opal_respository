using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class BaseEventConfiguration : BaseEntityTypeConfiguration<EventEntity>
    {
        public override void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder
                .Property(e => e.Type)
                .ConfigureForEnum()
                .HasDefaultValue(EventType.BaseEvent);

            builder
                .HasDiscriminator(e => e.Type)
                .HasValue<PersonalEvent>(EventType.Personal)
                .HasValue<CommunityEvent>(EventType.Community)
                .HasValue<EventEntity>(EventType.BaseEvent)
                .HasValue(EventType.BaseEvent);

            builder
                .Property(e => e.Title)
                .IsUnicode()
                .HasMaxLength(2000);

            builder
                .Property(e => e.Source)
                .ConfigureForEnum();

            builder
                .Property(e => e.RepeatFrequency)
                .ConfigureForEnum();

            builder
                .Property(e => e.Status)
                .ConfigureForEnum();

            builder
                .HasIndex(e => new { e.SourceId, e.Source })
                .IsUnique();
        }
    }
}
