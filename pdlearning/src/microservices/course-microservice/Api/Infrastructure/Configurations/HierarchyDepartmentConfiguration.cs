using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class HierarchyDepartmentConfiguration : BaseConfiguration<HierarchyDepartment>
    {
        public override void Configure(EntityTypeBuilder<HierarchyDepartment> builder)
        {
            base.Configure(builder);
            builder.ToTable("HierarchyDepartments");

            builder.HasIndex(p => p.ParentId);
            builder.HasIndex(p => p.HierarchyDepartmentId);
            builder.HasIndex(p => p.Path);
        }
    }
}
