using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SyncedCslCommunityMemberConfiguration : BaseEntityConfiguration<SyncedCslCommunityMember>
    {
        public override void Configure(EntityTypeBuilder<SyncedCslCommunityMember> builder)
        {
            builder.Property(_ => _.Role)
                .IsUnicode(false)
                .HasMaxLength(10)
                .IsFixedLength(false);

            builder.HasIndex(_ => new { _.CommunityId, _.UserId });
            base.Configure(builder);
        }
    }
}
