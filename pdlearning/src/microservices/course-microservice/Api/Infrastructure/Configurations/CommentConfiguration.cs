using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CommentConfiguration : BaseConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            base.Configure(builder);

            builder.HasIndex(p => new { p.UserId, p.CreatedDate });
            builder.HasIndex(p => new { p.ObjectId, p.CreatedDate });
            builder.HasIndex(p => new { p.Action, p.CreatedDate });
            builder.HasIndex(p => p.CreatedDate);
        }
    }
}
