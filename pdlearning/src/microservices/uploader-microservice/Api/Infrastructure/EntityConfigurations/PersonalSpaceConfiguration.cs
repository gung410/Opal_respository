using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Uploader.Infrastructure.EntityConfigurations
{
    public class PersonalSpaceConfiguration : BaseEntityTypeConfiguration<Domain.Entities.PersonalSpace>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.PersonalSpace> builder)
        {
            builder.Property(e => e.IsStorageUnlimited).HasDefaultValue(false);
        }
    }
}
