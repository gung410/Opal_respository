using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class RegistrationECertificateConfiguration : BaseConfiguration<RegistrationECertificate>
    {
        public override void Configure(EntityTypeBuilder<RegistrationECertificate> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Base64Image).HasColumnType("varchar(max)");
            builder.Property(p => p.PdfFileName).HasColumnType("nvarchar(2000)");
            builder.Property(p => p.Base64Pdf).HasColumnType("varchar(max)");

            // Column Indexes
            builder.HasIndex(p => new { p.UserId, p.CreatedDate });
        }
    }
}
