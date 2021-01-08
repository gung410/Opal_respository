using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class FormConfiguration : BaseEntityConfiguration<Domain.Entities.Form>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.Form> builder)
        {
            builder
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<FormType>())
                .HasColumnType("varchar(30)");
            builder.Property(p => p.Title).HasMaxLength(Domain.Entities.Form.MaxTitleLength);
            builder.Property(p => p.RandomizedQuestions).HasDefaultValue(false);
            builder.Property(p => p.IsSurveyTemplate).HasDefaultValue(false);
            builder
                .Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<FormStatus>())
                .HasColumnType("varchar(30)");
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder
              .Property(e => e.SurveyType)
              .HasConversion(new EnumToStringConverter<FormSurveyType>())
              .HasColumnType("varchar(30)");
            builder
             .Property(e => e.AnswerFeedbackDisplayOption)
             .HasConversion(new EnumToStringConverter<AnswerFeedbackDisplayOption>())
             .HasColumnType("varchar(30)");
            builder
              .Property(e => e.SqRatingType)
              .HasConversion(new EnumToStringConverter<SqRatingType>())
              .HasColumnType("varchar(50)");

            builder
              .Property(e => e.StandaloneMode)
              .HasConversion(new EnumToStringConverter<FormStandaloneMode>())
              .HasColumnType("varchar(30)");

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
