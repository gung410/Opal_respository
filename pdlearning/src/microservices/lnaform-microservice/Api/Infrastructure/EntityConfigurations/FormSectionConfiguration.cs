using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.LnaForm.Infrastructure.EntityConfigurations
{
    public class FormSectionConfiguration : BaseEntityConfiguration<FormSection>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormSection> builder)
        {
            builder.Property(p => p.MainDescription).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.AdditionalDescription).HasColumnType("nvarchar(MAX)");

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.FormId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
