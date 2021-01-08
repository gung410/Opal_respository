using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserConfiguration : BaseEntityTypeConfiguration<CSL_User>
    {
        public override void Configure(EntityTypeBuilder<CSL_User> builder)
        {
            builder.ToTable("csl_User", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedAt).HasColumnType("datetime2(0)");

            builder.Property(e => e.Guid)
                .HasMaxLength(45)
                .IsUnicode(false);

            builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(0)");

            builder.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
