using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class UserPersonalEventConfiguration : BaseEntityTypeConfiguration<UserPersonalEvent>
    {
        public override void Configure(EntityTypeBuilder<UserPersonalEvent> builder)
        {
            builder.HasIndex(p => new { p.UserId })
                .IncludeProperties(ue => new
                {
                    ue.EventId,
                    ue.IsAccepted
                });

            builder.HasIndex(p => new { p.EventId, p.UserId })
                .IsUnique();

            builder.HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(ue => ue.EventId);
        }
    }
}
