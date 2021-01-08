using System;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Entities;

#pragma warning disable SA1124
namespace Thunder.Platform.Core.Domain.Repositories
{
    /// <summary>
    /// This interface must be implemented by all repositories to identify them by convention.
    /// Implement generic version instead of this one.
    /// </summary>
    public interface IRepository : ITransientInstance
    {
    }

    /// <summary>
    /// A shortcut of <see cref="IRepository{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>
    {
    }

    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity.</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository, IReadOnlyRepository<TEntity, TPrimaryKey>, IWriteRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
    }
}
#pragma warning restore SA1124
