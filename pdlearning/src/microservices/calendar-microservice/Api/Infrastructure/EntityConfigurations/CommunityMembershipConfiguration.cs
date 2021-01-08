using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class CommunityMembershipConfiguration : BaseEntityTypeConfiguration<CommunityMembership>
    {
        public override void Configure(EntityTypeBuilder<CommunityMembership> builder)
        {
            builder
                .Property(e => e.Role)
                .ConfigureForEnum();

            builder
                .HasIndex(p => new { p.CommunityId, p.UserId })
                .IsUnique();
        }
    }
}
