using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class DepartmentTypeConfiguration : BaseEntityTypeConfiguration<DepartmentType>
    {
        public override void Configure(EntityTypeBuilder<DepartmentType> builder)
        {
            builder.Property(p => p.ExtId).HasMaxLength(200);

            builder.HasIndex(p => new { p.DepartmentTypeId, p.ExtId });
            builder.HasIndex(p => new { p.ExtId, p.DepartmentTypeId });
        }
    }
}
