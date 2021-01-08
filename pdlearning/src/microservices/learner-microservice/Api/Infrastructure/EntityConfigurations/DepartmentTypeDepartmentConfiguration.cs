using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class DepartmentTypeDepartmentConfiguration : BaseEntityTypeConfiguration<DepartmentTypeDepartment>
    {
        public override void Configure(EntityTypeBuilder<DepartmentTypeDepartment> builder)
        {
            builder.HasIndex(p => new { p.DepartmentTypeId, p.DepartmentId });
            builder.HasIndex(p => new { p.DepartmentId, p.DepartmentTypeId });
        }
    }
}
