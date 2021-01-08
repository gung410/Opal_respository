using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_LikeCommentConfiguration : BaseEntityTypeConfiguration<CSL_LikeComment>
    {
        public override void Configure(EntityTypeBuilder<CSL_LikeComment> builder)
        {
            builder.ToTable("csl_LikeComment", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);
        }
    }
}
