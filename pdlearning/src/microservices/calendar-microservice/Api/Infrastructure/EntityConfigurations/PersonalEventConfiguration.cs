using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class PersonalEventConfiguration : BaseEntityTypeConfiguration<PersonalEvent>
    {
        public override void Configure(EntityTypeBuilder<PersonalEvent> builder)
        {
            builder.HasBaseType<EventEntity>();
        }
    }
}
