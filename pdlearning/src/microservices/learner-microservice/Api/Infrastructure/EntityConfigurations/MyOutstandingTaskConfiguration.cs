using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyOutstandingTaskConfiguration : BaseEntityTypeConfiguration<MyOutstandingTask>
    {
        public override void Configure(EntityTypeBuilder<MyOutstandingTask> builder)
        {
            builder.Property(e => e.ItemType)
                .HasConversion(new EnumToStringConverter<OutstandingTaskType>())
                .ConfigureForEnum();

            builder.HasIndex(p => new { p.UserId, p.Priority });
            builder.HasIndex(p => new { p.ItemType, p.ItemId });
            builder.HasIndex(p => new { p.UserId, p.ItemType, p.ItemId });
        }
    }
}
