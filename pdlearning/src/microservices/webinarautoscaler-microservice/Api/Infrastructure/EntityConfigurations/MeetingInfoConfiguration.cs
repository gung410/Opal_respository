using Microservice.WebinarAutoscaler.Application.Models;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.WebinarAutoscaler.Infrastructure.EntityConfigurations
{
    public class MeetingInfoConfiguration : BaseEntityTypeConfiguration<MeetingInfo>
    {
        public override void Configure(EntityTypeBuilder<MeetingInfo> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(e => e.Title)
                .HasMaxLength(2500);
        }
    }
}
