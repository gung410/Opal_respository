using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_EcertificateTemplateConfiguration : BaseEntityTypeConfiguration<CAM_EcertificateTemplate>
    {
        public override void Configure(EntityTypeBuilder<CAM_EcertificateTemplate> builder)
        {
            builder.HasKey(e => e.EcertificateTemplateId);

            builder.ToTable("cam_ECertificateTemplate", "staging");

            builder.Property(e => e.EcertificateTemplateId)
                .HasColumnName("ECertificateTemplateId")
                .ValueGeneratedNever();
        }
    }
}
