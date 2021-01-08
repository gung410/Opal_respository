using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class TeamAccessSharingConfiguration : BaseEntityTypeConfiguration<TeamAccessSharing>
    {
        public override void Configure(EntityTypeBuilder<TeamAccessSharing> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.OwnerId);
            builder.HasIndex(a => a.UserId);
        }
    }
}
