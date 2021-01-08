using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowUserConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowUser>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowUser> builder)
        {
            builder.ToTable("csl_UserFollowUser", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);
        }
    }
}
