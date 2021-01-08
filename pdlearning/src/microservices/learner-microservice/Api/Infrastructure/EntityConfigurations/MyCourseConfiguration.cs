using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class MyCourseConfiguration : BaseEntityTypeConfiguration<MyCourse>
    {
        public override void Configure(EntityTypeBuilder<MyCourse> builder)
        {
            builder.Property(e => e.ReviewStatus)
                .HasMaxLength(MyCourse.MaxReviewStatusLength);

            builder.Property(e => e.Version)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<MyCourseStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.CourseType)
                .HasConversion(new EnumToStringConverter<LearningCourseType>())
                .HasColumnType("varchar(30)");

            builder.Property(e => e.MyRegistrationStatus)
                .HasConversion(new EnumToStringConverter<RegistrationStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.MyWithdrawalStatus)
                .HasConversion(new EnumToStringConverter<WithdrawalStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.DisplayStatus)
                .HasConversion(new EnumToStringConverter<DisplayStatus>())
                .HasColumnType("varchar(50)");

            builder.Property(e => e.Version)
                .HasColumnType("varchar(5)");

            // Index from here!!
            // TIPs: DO NOT create too many indexes. It will create pressure on CUD operators.
            //       Recommend to create indexes on columns that have low chances to be changed.
            builder.HasIndex(mc => mc.UserId);

            // In some cases the query plan optimizer of sql will choose the best one base on statistics of index
            // So it's not redundant when we have two index that look like the same here.
            builder.HasIndex(mc => new { mc.UserId, mc.CourseId });
            builder.HasIndex(mc => new { mc.CourseId, mc.UserId });
        }
    }
}
