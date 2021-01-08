using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Webinar.Infrastructure.EntityConfigurations
{
    public class AttendeeConfiguration : BaseEntityTypeConfiguration<Attendee>
    {
        public override void Configure(EntityTypeBuilder<Attendee> builder)
        {
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.MeetingId);
        }
    }
}
