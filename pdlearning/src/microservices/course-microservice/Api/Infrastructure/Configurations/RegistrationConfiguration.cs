using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class RegistrationConfiguration : BaseConfiguration<Registration>
    {
        public override void Configure(EntityTypeBuilder<Registration> builder)
        {
            base.Configure(builder);
            builder.Ignore(x => x.IsParticipant);

            builder.Property(e => e.RegistrationType)
                .HasConversion(new EnumToStringConverter<RegistrationType>())
                .HasDefaultValue(RegistrationType.Application)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<RegistrationStatus>())
               .HasDefaultValue(RegistrationStatus.PendingConfirmation)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.WithdrawalStatus)
               .HasConversion(new EnumToStringConverter<WithdrawalStatus>())
               .IsRequired(false)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.ClassRunChangeStatus)
               .HasConversion(new EnumToStringConverter<ClassRunChangeStatus>())
               .IsRequired(false)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.LearningStatus)
               .HasConversion(new EnumToStringConverter<LearningStatus>())
               .HasDefaultValue(LearningStatus.NotStarted)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.CourseCriteriaViolation)
               .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<CourseCriteriaLearnerViolation>(v, null) : null);

            builder.HasOne(p => p.ECertificate).WithOne(p => p.Participant)
                .HasForeignKey<RegistrationECertificate>(p => p.Id);

            builder.HasIndex(p => new { p.ClassRunChangeId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.WithdrawalStatus, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunChangeStatus, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunChangeId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseCriteriaViolated, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsExpired, p.IsDeleted, p.CreatedDate });
        }
    }
}
