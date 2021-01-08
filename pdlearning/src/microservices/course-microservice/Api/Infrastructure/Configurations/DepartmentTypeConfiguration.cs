using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class DepartmentTypeConfiguration : BaseConfiguration<DepartmentType>
    {
        public override void Configure(EntityTypeBuilder<DepartmentType> builder)
        {
            base.Configure(builder);
            builder.ToTable("DepartmentTypes");

            builder.HasIndex(p => p.DepartmentTypeId);
            builder.HasIndex(p => p.ExtId);
        }
    }
}
