using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserFollowingConfiguration : BaseEntityTypeConfiguration<UserFollowing>
    {
        public override void Configure(EntityTypeBuilder<UserFollowing> builder)
        {
            builder.HasIndex(p => new { p.UserId, p.FollowingUserId, p.CreatedDate });
        }
    }
}
