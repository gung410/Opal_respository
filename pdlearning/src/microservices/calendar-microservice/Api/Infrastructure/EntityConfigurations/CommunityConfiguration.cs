using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class CommunityConfiguration : BaseEntityTypeConfiguration<Community>
    {
        public override void Configure(EntityTypeBuilder<Community> builder)
        {
            builder.HasMany(c => c.CommunityEvents)
                .WithOne(ce => ce.Community)
                .HasForeignKey(ce => ce.CommunityId)
                .HasPrincipalKey(c => c.Id);

            builder.HasIndex(c => c.Status);

            builder.Property(c => c.Status)
                .ConfigureForEnum();
        }
    }
}
