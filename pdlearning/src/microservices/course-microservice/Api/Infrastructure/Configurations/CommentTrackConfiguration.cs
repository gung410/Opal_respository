using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CommentTrackConfiguration : BaseConfiguration<CommentTrack>
    {
        public override void Configure(EntityTypeBuilder<CommentTrack> builder)
        {
            base.Configure(builder);

            builder.HasIndex(p => new { p.UserId, p.CreatedDate });
            builder.HasIndex(p => p.CreatedDate);
        }
    }
}
