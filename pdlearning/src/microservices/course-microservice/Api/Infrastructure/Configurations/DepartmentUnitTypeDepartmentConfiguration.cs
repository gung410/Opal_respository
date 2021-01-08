using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class DepartmentUnitTypeDepartmentConfiguration : BaseConfiguration<DepartmentUnitTypeDepartment>
    {
        public override void Configure(EntityTypeBuilder<DepartmentUnitTypeDepartment> builder)
        {
            base.Configure(builder);
            builder.ToTable("DepartmentUnitTypeDepartment");

            builder.HasIndex(p => p.DepartmentId);
            builder.HasIndex(p => new { p.DepartmentUnitTypeId, p.IsDeleted, p.CreatedDate });
        }
    }
}
