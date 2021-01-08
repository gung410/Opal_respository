using Microservice.Uploader.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Uploader.Infrastructure.EntityConfigurations
{
    public class PersonalFileConfiguration : BaseEntityTypeConfiguration<Domain.Entities.PersonalFile>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.PersonalFile> builder)
        {
            builder.Property(e => e.FileName).HasMaxLength(255);
            builder.Property(e => e.FileLocation).HasMaxLength(1000);
            builder.Property(e => e.FileExtension).HasMaxLength(10).IsUnicode(false);
            builder
             .Property(e => e.FileType)
             .HasConversion(new EnumToStringConverter<FileType>())
             .HasColumnType("varchar(30)");
        }
    }
}
