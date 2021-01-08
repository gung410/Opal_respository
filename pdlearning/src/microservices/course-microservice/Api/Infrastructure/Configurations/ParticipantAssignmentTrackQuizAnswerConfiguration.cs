using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ParticipantAssignmentTrackQuizAnswerConfiguration : BaseConfiguration<ParticipantAssignmentTrackQuizAnswer>
    {
        public override void Configure(EntityTypeBuilder<ParticipantAssignmentTrackQuizAnswer> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => x.QuizAssignmentFormId);
            builder.HasIndex(x => x.IsDeleted);
        }
    }
}
