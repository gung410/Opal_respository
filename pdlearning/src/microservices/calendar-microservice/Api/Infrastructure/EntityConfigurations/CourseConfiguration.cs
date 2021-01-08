using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Calendar.Infrastructure.EntityConfigurations
{
    public class CourseConfiguration : BaseEntityTypeConfiguration<Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(e => e.Status)
                .ConfigureForEnum();
        }
    }
}
