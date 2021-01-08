using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class UserMetadataConfiguration : BaseConfiguration<UserMetadata>
    {
        public override void Configure(EntityTypeBuilder<UserMetadata> builder)
        {
            base.Configure(builder);

            builder.HasOne(p => p.CourseUser)
                .WithMany(p => p.UserMetadatas)
                .HasForeignKey(p => p.UserId);

            builder.Property(p => p.Type)
                .HasConversion(new EnumToStringConverter<UserMetadataValueType>())
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Value);
        }
    }
}
