using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserSharingDetailConfiguration : BaseEntityTypeConfiguration<UserSharingDetail>
    {
        public override void Configure(EntityTypeBuilder<UserSharingDetail> builder)
        {
        }
    }
}
