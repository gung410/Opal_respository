using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_QuizAssignmentFormConfiguration : BaseEntityTypeConfiguration<CAM_QuizAssignmentForm>
    {
        public override void Configure(EntityTypeBuilder<CAM_QuizAssignmentForm> builder)
        {
            builder.HasKey(e => e.QuizAssignmentFormId);

            builder.ToTable("cam_QuizAssignmentForm", "staging");

            builder.Property(e => e.QuizAssignmentFormId).ValueGeneratedNever();
        }
    }
}
