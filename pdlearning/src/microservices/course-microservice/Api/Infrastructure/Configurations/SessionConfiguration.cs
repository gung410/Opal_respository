using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class SessionConfiguration : BaseConfiguration<Session>
    {
        public override void Configure(EntityTypeBuilder<Session> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.SessionCode)
               .HasMaxLength(25);

            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
        }
    }
}
