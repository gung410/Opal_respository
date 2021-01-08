using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class QuizAssignmentFormConfiguration : BaseConfiguration<QuizAssignmentForm>
    {
        public override void Configure(EntityTypeBuilder<QuizAssignmentForm> builder)
        {
            base.Configure(builder);
            builder.HasMany(x => x.Answers).WithOne(x => x.QuizAssignmentForm)
                .HasForeignKey(x => x.QuizAssignmentFormId);
            builder.HasIndex(p => p.IsDeleted);
        }
    }
}
