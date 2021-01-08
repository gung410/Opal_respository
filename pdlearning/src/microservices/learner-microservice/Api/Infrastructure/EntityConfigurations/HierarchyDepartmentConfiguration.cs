using Conexus.Opal.AccessControl.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class HierarchyDepartmentConfiguration : BaseEntityTypeConfiguration<HierarchyDepartment>
    {
        public override void Configure(EntityTypeBuilder<HierarchyDepartment> builder)
        {
            builder.HasIndex(p => p.ParentId);
            builder.HasIndex(p => p.HierarchyDepartmentId);
            builder.HasIndex(p => new { p.Path, p.DepartmentId });
        }
    }
}
