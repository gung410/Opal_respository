using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LanguageMap.
    /// </summary>
    public class LanguageMap : IEntityTypeConfiguration<LanguageEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LanguageEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LanguageId);

            builder.Property(t => t.LanguageCode)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(t => t.Dir)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.NativeName)
                .IsRequired()
                .HasMaxLength(64);

            // Table & Column Mappings
            builder.ToTable("Language", "dbo");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.LanguageCode).HasColumnName("LanguageCode");
            builder.Property(t => t.Dir).HasColumnName("Dir");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.NativeName).HasColumnName("NativeName");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}
