using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.LnaForm.Infrastructure.EntityConfigurations
{
    public class FormConfiguration : BaseEntityConfiguration<Domain.Entities.Form>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.Form> builder)
        {
            builder.Property(p => p.Title).HasMaxLength(Domain.Entities.Form.MaxTitleLength);
            builder
                .Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<FormStatus>())
                .HasColumnType("varchar(30)");
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder
              .Property(e => e.SqRatingType)
              .HasConversion(new EnumToStringConverter<SqRatingType>())
              .HasColumnType("varchar(50)");

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
