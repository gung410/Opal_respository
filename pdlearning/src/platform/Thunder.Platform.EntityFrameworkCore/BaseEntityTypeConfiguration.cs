using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Thunder.Platform.EntityFrameworkCore
{
    /// <summary>
    /// Base class for entity configuration. The configuration should be outside of the application context.
    /// Therefor it should not include any DI, configuration, etc., just pure logic supported by EF Core.
    /// </summary>
    /// <typeparam name="TEntityType">The type of an entity.</typeparam>
    public abstract class BaseEntityTypeConfiguration<TEntityType> : IEntityTypeConfiguration<TEntityType>
        where TEntityType : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntityType> builder);
    }
}
