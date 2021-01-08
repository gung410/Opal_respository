using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class AssessmentConfiguration : BaseConfiguration<AssessmentAnswer>
    {
        public override void Configure(EntityTypeBuilder<AssessmentAnswer> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.CriteriaAnswers)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<AssessmentCriteriaAnswer>>(v, null) : null);

            builder.HasIndex(p => new { p.AssessmentId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ParticipantAssignmentTrackId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
        }
    }
}
