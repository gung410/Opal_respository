using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class CalendarUserConfiguration : BaseEntityTypeConfiguration<CalendarUser>
    {
        public override void Configure(EntityTypeBuilder<CalendarUser> builder)
        {
            builder.HasIndex(u => u.PrimaryApprovalOfficerId);
            builder.HasIndex(u => u.AlternativeApprovalOfficerId);
        }
    }
}
