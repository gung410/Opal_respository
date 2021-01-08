using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Thunder.Platform.EntityFrameworkCore.Tests.Dummy
{
    public class SampleConfiguration : BaseEntityTypeConfiguration<EmployeeEntity>
    {
        public override void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
        }
    }
}
