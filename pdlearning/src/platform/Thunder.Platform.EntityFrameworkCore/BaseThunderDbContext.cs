using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.EntityFrameworkCore.Logging;

namespace Thunder.Platform.EntityFrameworkCore
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class BaseThunderDbContext : DbContext
    {
        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(BaseThunderDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        public string Id { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// The connection string to create database context.
        /// This property was set by <see cref="InitializeConnectionString"/> when resolving dbcontext <see cref="IDbContextResolver"/>.
        /// If this connection string is not defined, the resolving process will use <see cref="IConnectionStringResolver"/>.
        /// </summary>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// The database connection to create database context. This is usually used for the transaction processing.
        /// This property was set by <see cref="InitializeConnectionString"/> when resolving dbcontext <see cref="IDbContextResolver"/>.
        /// If this connection string is not defined, the resolving process will use <see cref="IConnectionStringResolver"/>.
        /// </summary>
        protected DbConnection DbConnection { get; private set; }

        [Experimental("Property injection by thunder framework.")]
        protected IQueryTrackingSource QueryTrackingSource { get; private set; }

        public void InitializeConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void InitializeDiInstances(IQueryTrackingSource queryTrackingSource)
        {
            QueryTrackingSource = queryTrackingSource;
        }

        public void InitializeDbConnection(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public override int SaveChanges()
        {
            ApplyAuditingMechanism();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ApplyAuditingMechanism();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (QueryTrackingSource != null)
            {
                optionsBuilder.AddInterceptors(new QueryTrackingInterceptor(QueryTrackingSource));
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply query filter.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }

            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return entity => ((ISoftDelete)entity).IsDeleted == false;
            }

            return null;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        private void ApplyAuditingMechanism()
        {
            var entries = ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        ApplyCreationAuditMechanism(entry.Entity);
                        break;

                    case EntityState.Modified:
                        ApplyModificationAuditMechanism(entry.Entity);
                        break;

                    case EntityState.Deleted:
                        if (IsSoftDelete(entry.Entity))
                        {
                            CancelDeletionForSoftDelete(entry);
                            ApplySoftDeleteMechanism(entry.Entity);
                        }

                        break;

                    case EntityState.Detached:
                        break;

                    case EntityState.Unchanged:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool IsSoftDelete(object entityAsObj)
        {
            return entityAsObj is ISoftDelete;
        }

        private void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete softDeleteEntity))
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            softDeleteEntity.IsDeleted = true;
        }

        private void ApplySoftDeleteMechanism(object entityAsObj)
        {
            if (!(entityAsObj is IHasDeletedDate entity))
            {
                return;
            }

            entity.DeletedDate = Clock.Now;
        }

        private void ApplyCreationAuditMechanism(object entityAsObj)
        {
            if (!(entityAsObj is IHasCreatedDate entity))
            {
                return;
            }

            entity.CreatedDate = Clock.Now;

            /* The ChangedDate was set same as CreatedDate at creation time in order to support sort by last modified value with only ChangedDate property.*/
            if (entityAsObj is IHasChangedDate modificationEntity)
            {
                modificationEntity.ChangedDate = entity.CreatedDate;
            }
        }

        private void ApplyModificationAuditMechanism(object entityAsObj)
        {
            if (!(entityAsObj is IHasChangedDate entity))
            {
                return;
            }

            entity.ChangedDate = Clock.Now;
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
