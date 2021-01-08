using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class FormConfiguration : BaseEntityTypeConfiguration<Form>
    {
        public override void Configure(EntityTypeBuilder<Form> builder)
        {
            builder
                .Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<FormType>())
                .HasColumnType("varchar(30)");
            builder.Property(p => p.Title).HasMaxLength(1000);
            builder
                .Property(e => e.Status)
                .HasConversion(new EnumToStringConverter<FormStatus>())
                .HasColumnType("varchar(30)");

            builder
                .Property(e => e.SurveyType)
                .HasConversion(new EnumToStringConverter<FormSurveyType>())
                .HasColumnType("varchar(30)");

            builder
                .Property(e => e.StandaloneMode)
                .HasConversion(new EnumToStringConverter<FormStandaloneMode>())
                .HasColumnType("varchar(30)");
        }
    }
}
