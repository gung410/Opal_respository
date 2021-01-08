using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class FormQuestionConfiguration : BaseEntityConfiguration<FormQuestion>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormQuestion> builder)
        {
            builder.Property(p => p.Priority).HasDefaultValue(0);
            builder.Property(p => p.IsScoreEnabled).HasDefaultValue(true);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder.Property(p => p.Title).HasMaxLength(20000).HasColumnType("NTEXT");

            QuestionConfigurationHelper.ConfigOwnQuestionEntity<FormQuestion>(builder);
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.FormId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
