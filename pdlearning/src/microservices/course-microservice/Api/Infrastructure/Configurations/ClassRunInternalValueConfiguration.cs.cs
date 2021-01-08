using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ClassRunInternalValueConfiguration : BaseEntityTypeConfiguration<ClassRunInternalValue>
    {
        public override void Configure(EntityTypeBuilder<ClassRunInternalValue> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.HasOne(p => p.ClassRun)
                .WithMany(p => p.ClassRunInternalValues)
                .HasForeignKey(p => p.ClassRunId);
            builder.Property(p => p.Type)
                   .HasConversion(new EnumToStringConverter<ClassRunInternalValueType>())
                   .HasDefaultValue(ClassRunInternalValueType.FacilitatorIds)
                   .HasMaxLength(50)
                   .IsUnicode(false);
            builder.HasIndex(p => p.ClassRunId);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Value);
        }
    }
}
