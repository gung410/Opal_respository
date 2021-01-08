using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CoursePlanningConfiguration : BaseConfiguration<CoursePlanningCycle>
    {
        public override void Configure(EntityTypeBuilder<CoursePlanningCycle> builder)
        {
            base.Configure(builder);

            builder.HasMany(p => p.BlockoutDates).WithOne(p => p.PlanningCycle).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.IsConfirmedBlockoutDate)
                .HasDefaultValue(false);

            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.StartDate);
            builder.HasIndex(p => p.EndDate);
            builder.HasIndex(p => new { p.YearCycle, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsConfirmedBlockoutDate, p.IsDeleted, p.CreatedDate });
        }
    }
}
