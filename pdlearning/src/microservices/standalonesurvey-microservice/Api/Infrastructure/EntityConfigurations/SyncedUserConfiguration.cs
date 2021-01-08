using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SyncedUserConfiguration : BaseEntityTypeConfiguration<SyncedUser>
    {
        public override void Configure(EntityTypeBuilder<SyncedUser> builder)
        {
            // do nothing, intend to use in future.
        }
    }
}
