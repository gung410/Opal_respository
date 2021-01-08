using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Webinar.Infrastructure.EntityConfigurations
{
    public class BookingConfiguration : BaseEntityTypeConfiguration<Booking>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(e => e.Source)
                .ConfigureForEnum();
            builder.HasIndex(r => r.MeetingId);
        }
    }
}
