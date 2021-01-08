using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyLearningPackageConfiguration : BaseEntityTypeConfiguration<MyLearningPackage>
    {
        public override void Configure(EntityTypeBuilder<MyLearningPackage> builder)
        {
            builder.Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<LearningPackageType>())
                .HasColumnType("varchar(30)");
        }
    }
}
