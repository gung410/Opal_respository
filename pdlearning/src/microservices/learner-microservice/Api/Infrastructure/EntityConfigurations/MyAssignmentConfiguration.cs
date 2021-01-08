using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyAssignmentConfiguration : BaseEntityTypeConfiguration<MyAssignment>
    {
        public override void Configure(EntityTypeBuilder<MyAssignment> builder)
        {
            builder.Property(a => a.Status)
                .HasConversion(new EnumToStringConverter<MyAssignmentStatus>())
                .HasColumnType("varchar(50)");
        }
    }
}
