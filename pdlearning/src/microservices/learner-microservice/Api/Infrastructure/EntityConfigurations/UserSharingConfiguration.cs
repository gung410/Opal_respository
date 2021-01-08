using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserSharingConfiguration : BaseEntityTypeConfiguration<UserSharing>
    {
        public override void Configure(EntityTypeBuilder<UserSharing> builder)
        {
            builder.Property(e => e.ItemType)
                .HasConversion(new EnumToStringConverter<SharingType>())
                .HasColumnType("varchar(50)");
        }
    }
}
