using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CourseDepartmentConfiguration : BaseConfiguration<CourseDepartment>
    {
        public override void Configure(EntityTypeBuilder<CourseDepartment> builder)
        {
            base.Configure(builder);
            builder.ToTable("Departments");

            builder.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

            builder.Property(e => e.Name).HasMaxLength(256);

            builder.HasIndex(prop => prop.DepartmentId);
        }
    }
}
