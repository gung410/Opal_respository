using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ParticipantAssignmentTrackConfiguration : BaseConfiguration<ParticipantAssignmentTrack>
    {
        public override void Configure(EntityTypeBuilder<ParticipantAssignmentTrack> builder)
        {
            base.Configure(builder);
            builder.ToTable("ParticipantAssignmentTrack");
            builder.HasOne(x => x.QuizAnswer).WithOne(x => x.ParticipantAssignmentTrack)
                .HasForeignKey<ParticipantAssignmentTrackQuizAnswer>(x => x.Id);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<ParticipantAssignmentTrackStatus>())
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.HasIndex(p => new { p.RegistrationId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.AssignmentId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.SubmittedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.AssignedDate });
            builder.HasIndex(p => p.StartDate);
            builder.HasIndex(p => p.EndDate);
        }
    }
}
