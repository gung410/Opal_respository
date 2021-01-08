using Microservice.Form.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public abstract class BaseEntityConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder
                .Property(e => e.ExternalId)
                .HasColumnType("varchar(255)");
        }
    }
}
