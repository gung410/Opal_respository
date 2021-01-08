using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class CommunityEventConfiguration : BaseEntityTypeConfiguration<CommunityEvent>
    {
        public override void Configure(EntityTypeBuilder<CommunityEvent> builder)
        {
            builder.Property(e => e.CommunityEventPrivacy)
                .ConfigureForEnum();

            builder.HasBaseType<EventEntity>();
        }
    }
}
