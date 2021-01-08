using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class DepartmentTypeDepartmentConfiguration : BaseConfiguration<DepartmentTypeDepartment>
    {
        public override void Configure(EntityTypeBuilder<DepartmentTypeDepartment> builder)
        {
            base.Configure(builder);
            builder.ToTable("DepartmentTypeDepartments");

            builder.HasIndex(p => p.DepartmentId);
            builder.HasIndex(p => p.DepartmentTypeId);
        }
    }
}
