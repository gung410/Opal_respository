using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure.Configurations
{
    public abstract class BaseConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity> where TEntity : Entity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
