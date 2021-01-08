using Microservice.Form.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class FormAnswerAttachmentConfiguration : BaseEntityConfiguration<Domain.Entities.FormAnswerAttachment>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormAnswerAttachment> builder)
        {
            builder.Property(e => e.FileName).HasMaxLength(255);
            builder.Property(e => e.FileLocation).HasMaxLength(1000);
            builder.Property(e => e.FileExtension).HasMaxLength(10).IsUnicode(false);
            builder.Property(e => e.FileType)
            .HasConversion(new EnumToStringConverter<FileType>())
            .HasColumnType("varchar(30)");
            builder.HasIndex(p => new { p.FormQuestionAnswerId });

            base.Configure(builder);
        }
    }
}
