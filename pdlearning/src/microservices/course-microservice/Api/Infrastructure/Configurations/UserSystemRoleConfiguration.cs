using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class UserSystemRoleConfiguration : BaseConfiguration<UserSystemRole>
    {
        public override void Configure(EntityTypeBuilder<UserSystemRole> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Value)
               .HasMaxLength(100);

            builder.HasOne(p => p.CourseUser)
                .WithMany(p => p.UserSystemRoles)
                .HasForeignKey(p => p.UserId);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Value);
        }
    }
}
