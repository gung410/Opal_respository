using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities.Mapping
{
    public class FileInfoMap : IEntityTypeConfiguration<FileInfoEntity>
    {
        public void Configure(EntityTypeBuilder<FileInfoEntity> builder)
        {
            builder.ToTable("FileInfo", "dbo");
            builder.HasKey(t => new { t.FileInfoId });

            builder.Property(t => t.FileName).IsRequired(true);
            builder.Property(t => t.FilePath).IsRequired(true);
            builder.Property(t => t.OriginalFileName).IsRequired(true);
            builder.Property(t => t.Type).IsRequired(true);
            builder.Property(t => t.NumberOfRecord).IsRequired(true);
            builder.Property(t => t.FileTarget).IsRequired(true);
            builder.Property(t => t.CreatedDate).IsRequired(true);
            builder.Property(t => t.UserGuid).IsRequired(true);
        }
    }
}
