using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class SharedQuestionConfiguration : BaseEntityConfiguration<SharedQuestion>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.SharedQuestion> builder)
        {
            QuestionConfigurationHelper.ConfigOwnQuestionEntity<SharedQuestion>(builder);

            base.Configure(builder);
        }
    }
}
