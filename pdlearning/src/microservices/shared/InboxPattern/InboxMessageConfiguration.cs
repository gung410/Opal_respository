using Conexus.Opal.InboxPattern.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Conexus.Opal.Shared.InboxPattern
{
    public class InboxMessageConfiguration : BaseEntityTypeConfiguration<InboxMessage>
    {
        public override void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            builder.Property(e => e.Status)
                .ConfigureForEnum();

            builder.Property(e => e.ReadyToDelete)
                .HasDefaultValue(false);

            builder.HasIndex(e => new { e.Status });

            builder.HasIndex(e => new { e.CreatedDate, e.Status });

            builder.HasIndex(e => e.MessageId);
        }
    }
}
