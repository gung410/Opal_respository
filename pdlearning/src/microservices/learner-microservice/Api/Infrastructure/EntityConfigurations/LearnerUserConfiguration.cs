using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.Configurations
{
    public class LearnerUserConfiguration : BaseEntityTypeConfiguration<LearnerUser>
    {
        public override void Configure(EntityTypeBuilder<LearnerUser> builder)
        {
            builder.Property(p => p.OriginalUserId)
                .HasColumnName("UserID");

            builder.Property(p => p.DepartmentId)
                .HasColumnName("DepartmentID");

            builder.Property(p => p.AccountType)
                .HasConversion(new EnumToStringConverter<LearnerUserAccountType>())
                .HasMaxLength(100)
                .HasDefaultValue(LearnerUserAccountType.Internal)
                .IsUnicode(false);

            builder.Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<LearnerUserStatus>())
                .HasMaxLength(100)
                .HasDefaultValue(LearnerUserStatus.New)
                .IsUnicode(false);

            builder.HasIndex(p => p.OriginalUserId);
            builder.HasIndex(p => p.DepartmentId);
            builder.HasIndex(p => p.PrimaryApprovingOfficerId);
            builder.HasIndex(p => p.AlternativeApprovingOfficerId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => new { p.Status, p.FirstName });
            builder.HasIndex(p => new { p.Status, p.LastName });
            builder.HasIndex(p => new { p.Status, p.LastName, p.FirstName });
            builder.HasIndex(p => p.FirstName);
            builder.HasIndex(p => p.LastName);
            builder.HasIndex(p => p.Email);
            builder.HasIndex(p => new { p.Email, p.Status, p.FirstName });
            builder.HasIndex(p => new { p.Email, p.Status, p.LastName });
        }
    }
}
