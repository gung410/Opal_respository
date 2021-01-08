using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyClassRunConfiguration : BaseEntityTypeConfiguration<MyClassRun>
    {
        public override void Configure(EntityTypeBuilder<MyClassRun> builder)
        {
            builder.Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<RegistrationStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.WithdrawalStatus)
                .HasConversion(new EnumToStringConverter<WithdrawalStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.RegistrationType)
                .HasConversion(new EnumToStringConverter<RegistrationType>())
                .HasDefaultValue(RegistrationType.Application)
                .HasColumnType("varchar(50)");

            builder.Property(e => e.ClassRunChangeStatus)
                .HasConversion(new EnumToStringConverter<ClassRunChangeStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.LearningStatus)
                .HasConversion(new EnumToStringConverter<LearningStatus>())
                .HasDefaultValue(LearningStatus.NotStarted)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Limited to many MyClassRuns
            // Avoid duplicate data come when the message can redeliver in RabbitMQ
            builder.HasIndex(p => p.RegistrationId).IsUnique();
            builder.HasIndex(p => new { p.CourseId, p.UserId, p.Status, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted });
            builder.HasIndex(p => new { p.ClassRunId, p.Status, p.IsExpired, p.IsDeleted });
        }
    }
}
