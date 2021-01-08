using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class DigitalContentConfiguration : BaseEntityTypeConfiguration<DigitalContent>
    {
        public override void Configure(EntityTypeBuilder<DigitalContent> builder)
        {
            builder.Property(e => e.Title)
                .HasMaxLength(255);

            builder.Property(e => e.Type)
                .ConfigureForEnum();

            builder.Property(e => e.Status)
                .ConfigureForEnum();

            builder.Property(e => e.FileExtension)
                .HasMaxLength(10)
                .IsUnicode(false);
        }
    }
}
