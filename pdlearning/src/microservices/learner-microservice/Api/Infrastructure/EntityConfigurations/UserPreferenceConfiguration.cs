using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserPreferenceConfiguration : BaseEntityTypeConfiguration<UserPreference>
    {
        public override void Configure(EntityTypeBuilder<UserPreference> builder)
        {
            builder.Ignore(_ => _.Value);

            builder
                .HasIndex(_ => new { _.UserId, _.Key })
                .IsUnique();
        }
    }
}
