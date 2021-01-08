using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserLikeConfiguration : BaseEntityTypeConfiguration<UserLike>
    {
        public override void Configure(EntityTypeBuilder<UserLike> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(e => e.ItemType)
                .ConfigureForEnum();
        }
    }
}
