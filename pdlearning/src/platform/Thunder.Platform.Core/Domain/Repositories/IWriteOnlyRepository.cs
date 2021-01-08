using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Thunder.Platform.Core.Domain.Entities;

#pragma warning disable SA1124
namespace Thunder.Platform.Core.Domain.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="IWriteRepository{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IWriteOnlyRepository<TEntity> : IWriteRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>
    {
    }

    /// <summary>
    /// This interface is implemented by all repositories (for write) to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on.</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity.</typeparam>
    public interface IWriteRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity.</param>
        /// <returns>The entity.</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Inserts new entities.
        /// </summary>
        /// <param name="entities">Inserted entities.</param>
        /// <returns>The entities.</returns>
        IEnumerable<TEntity> InsertMany(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// Inserts new entities.
        /// </summary>
        /// <param name="entities">Inserted entities.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Id of the entity.</returns>
        TPrimaryKey InsertAndGetId(TEntity entity);

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Id of the entity.</returns>
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>The entity.</returns>
        TEntity InsertOrUpdate(TEntity entity);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        /// <summary>
        /// Inserts or updates given entities depending on Id's value.
        /// </summary>
        /// <param name="entities">Entities.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<TEntity>> InsertOrUpdateManyAsync(List<TEntity> entities);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Id of the entity.</returns>
        TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Id of the entity.</returns>
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>The entity.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Updates existing entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        /// <returns>The entities.</returns>
        IEnumerable<TEntity> UpdateMany(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates existing entities.
        /// </summary>
        /// <param name="entities">Entities.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<TEntity>> UpdateManyAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity.</param>
        /// <param name="updateAction">Action that can be used to change values of the entity.</param>
        /// <returns>Updated entity.</returns>
        TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity.</param>
        /// <param name="updateAction">Action that can be used to change values of the entity.</param>
        /// <returns>Updated entity.</returns>
        Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes entities.
        /// </summary>
        /// <param name="entities">Entities to be deleted.</param>
        void DeleteMany(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Deletes entities.
        /// </summary>
        /// <param name="entities">Entities to be deleted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteManyAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity.</param>
        void Delete(TPrimaryKey id);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(TPrimaryKey id);

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
#pragma warning restore SA1124
