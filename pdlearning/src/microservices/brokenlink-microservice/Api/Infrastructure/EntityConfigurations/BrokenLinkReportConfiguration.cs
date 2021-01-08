using Microservice.BrokenLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.BrokenLink.Infrastructure.EntityConfigurations
{
    public class BrokenLinkReportConfiguration : BaseEntityTypeConfiguration<BrokenLinkReport>
    {
        public override void Configure(EntityTypeBuilder<BrokenLinkReport> builder)
        {
            builder.Property(e => e.Url).HasMaxLength(5000);

            builder.Property(e => e.Module)
                .ConfigureForEnum();

            builder.Property(e => e.ObjectTitle)
                .HasMaxLength(5000);

            builder.Property(e => e.ObjectOwnerName)
                .HasMaxLength(500);

            builder.Property(e => e.ObjectDetailUrl).
                HasMaxLength(2000);

            builder.Property(e => e.ContentType)
                .ConfigureForEnum();

            builder.HasIndex(p => new { p.OriginalObjectId, p.Module, p.CreatedDate })
                .IncludeProperties(p =>
                    new
                    {
                        p.Id,
                        p.ObjectId,
                        p.Url,
                        p.Description,
                        p.ReportBy,
                        p.ReporterName,
                        p.ContentType,
                        p.ObjectTitle,
                        p.ObjectOwnerId,
                        p.ObjectOwnerName,
                        p.ObjectDetailUrl,
                        p.ParentId
                    });

            builder.HasIndex(p => new { p.ReportBy, p.ContentType, p.Module, p.CreatedDate })
                .IncludeProperties(p =>
                    new
                    {
                        p.Id,
                        p.OriginalObjectId,
                        p.ObjectId,
                        p.Url,
                        p.ReporterName,
                        p.Description,
                        p.ObjectTitle,
                        p.ObjectOwnerId,
                        p.ObjectOwnerName,
                        p.ObjectDetailUrl,
                        p.ParentId
                    });
        }
    }
}
