using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SyncedCslCommunityConfiguration : BaseEntityConfiguration<SyncedCslCommunity>
    {
        public override void Configure(EntityTypeBuilder<SyncedCslCommunity> builder)
        {
            builder.Property(_ => _.Status)
                .IsUnicode(false)
                .HasMaxLength(10)
                .IsFixedLength(false);

            base.Configure(builder);
        }
    }
}
