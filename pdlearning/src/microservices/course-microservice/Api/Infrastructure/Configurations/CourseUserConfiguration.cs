using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CourseUserConfiguration : BaseConfiguration<CourseUser>
    {
        public override void Configure(EntityTypeBuilder<CourseUser> builder)
        {
            base.Configure(builder);
            builder.ToTable("Users");

            builder.Property(p => p.OriginalUserId)
                .HasColumnName("UserID");
            builder.Property(p => p.DepartmentId)
                .HasColumnName("DepartmentID");

            builder.HasMany(p => p.UserMetadatas)
                .WithOne(p => p.CourseUser)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.UserSystemRoles)
                .WithOne(p => p.CourseUser)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.AccountType)
                .HasConversion(new EnumToStringConverter<CourseUserAccountType>())
                .HasMaxLength(100)
                .HasDefaultValue(CourseUserAccountType.Internal)
                .IsUnicode(false);

            builder.Property(p => p.Status)
                .HasConversion(new EnumToStringConverter<CourseUserStatus>())
                .HasMaxLength(100)
                .HasDefaultValue(CourseUserStatus.New)
                .IsUnicode(false);

            builder.Property(e => e.Track)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.SystemRoles)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingLevel)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingCourseOfStudy)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingSubject)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.CocurricularActivity)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.DevelopmentalRole)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.JobFamily)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.EasSubstantiveGradeBanding)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.ServiceScheme)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.Designation)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningFramework)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.SystemRoles)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasIndex(p => p.OriginalUserId);
            builder.HasIndex(p => p.DepartmentId);
            builder.HasIndex(p => p.PrimaryApprovingOfficerId);
            builder.HasIndex(p => p.AlternativeApprovingOfficerId);
            builder.HasIndex(p => p.Status);

            // Technical Columns Indexes
            builder.HasIndex(p => p.FullTextSearchKey).IsUnique();
            builder.Property(e => e.FullTextSearch)
                .IsRequired(false);

            builder.Property(e => e.FullTextSearchKey)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValue(string.Empty);
        }
    }
}
