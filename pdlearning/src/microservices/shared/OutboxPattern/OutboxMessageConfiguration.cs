using Conexus.Opal.OutboxPattern;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Conexus.Opal.Shared.OutboxPattern
{
    public class OutboxMessageConfiguration : BaseEntityTypeConfiguration<OutboxMessage>
    {
        public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.Property(e => e.Status)
                .ConfigureForEnum();

            builder.Property(e => e.RoutingKey)
                .HasMaxLength(1000);

            builder.Property(e => e.Exchange)
                .HasMaxLength(1000);

            builder.Property(e => e.SourceIp)
                .HasMaxLength(255);

            builder.Property(e => e.UserId)
                .HasMaxLength(255);

            builder.Property(e => e.Status)
                .IsConcurrencyToken();

            builder.Property(e => e.ReadyToDelete)
                .IsConcurrencyToken();

            builder.Property(e => e.Timestamp)
                .IsRowVersion();

            builder.HasIndex(e => new { e.Status, e.CreatedDate });
        }
    }
}
