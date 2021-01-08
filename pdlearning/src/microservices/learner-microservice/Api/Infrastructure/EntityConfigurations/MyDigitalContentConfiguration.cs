using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyDigitalContentConfiguration : BaseEntityTypeConfiguration<MyDigitalContent>
    {
        public override void Configure(EntityTypeBuilder<MyDigitalContent> builder)
        {
            builder.Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<MyDigitalContentStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.ReviewStatus)
                .HasMaxLength(MyDigitalContent.MaxReviewStatusLength);

            builder.Property(e => e.Version)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.DigitalContentType)
                .HasConversion(new EnumToStringConverter<DigitalContentType>())
                .HasColumnType("varchar(50)");
        }
    }
}
