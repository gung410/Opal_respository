using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CommentViewPermissionConfiguration : BaseConfiguration<CommentViewPermission>
    {
        public override void Configure(EntityTypeBuilder<CommentViewPermission> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Id).HasDefaultValueSql("newid()");
            builder.Property(p => p.CreatedDate).HasDefaultValueSql("getdate()");
            builder.HasIndex(p => p.CanViewRole);
            builder.HasIndex(p => p.CommentAction);
            builder.HasIndex(p => p.CommentByUserRole);
            builder.HasIndex(p => p.CreatedDate);
            builder.HasIndex(p => p.ChangedDate);
        }
    }
}
