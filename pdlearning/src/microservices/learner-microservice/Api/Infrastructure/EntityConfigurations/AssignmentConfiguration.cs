using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class AssignmentConfiguration : BaseEntityTypeConfiguration<Assignment>
    {
        public override void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<AssignmentType>())
                .ConfigureForEnum();
        }
    }
}
