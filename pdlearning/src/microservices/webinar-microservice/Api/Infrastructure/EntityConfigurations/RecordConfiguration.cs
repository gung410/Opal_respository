using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Webinar.Infrastructure.EntityConfigurations
{
    public class RecordConfiguration : BaseEntityTypeConfiguration<Record>
    {
        public override void Configure(EntityTypeBuilder<Record> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.MeetingId);
            builder.Property(r => r.Status).ConfigureForEnum();
        }
    }
}
