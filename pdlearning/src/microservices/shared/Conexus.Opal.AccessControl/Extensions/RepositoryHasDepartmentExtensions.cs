using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Service.Authentication;

namespace Conexus.Opal.AccessControl.Extensions
{
    public static class RepositoryHasDepartmentExtensions
    {
        public static Guid? CurrentUserId(this IUserContext userContext)
        {
            var userId = userContext.GetValue<string>(CommonUserContextKeys.UserId);
            return !string.IsNullOrEmpty(userId) ? (Guid?)Guid.Parse(userId) : null;
        }

        public static IQueryable<TEntity> ApplyAccessControl<TEntity, TUserEntity>(
            this IQueryable<TEntity> query,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePermissionPredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            if (accessControlContext.UserContext.CurrentUserId() == null)
            {
                return query;
            }

            if (accessControlContext.UserContext.IsSysAdministrator())
            {
                return query;
            }

            if (!accessControlContext.UserContext.IsAdministrator())
            {
                return includePermissionPredicate != null ? query.Where(includePermissionPredicate) : query.Where(p => false);
            }
            else
            {
                var appliedAccessControlQuery = query
                .Join(
                    accessControlContext.GetInferiorDepartmentIds(),
                    item => item.DepartmentId,
                    departmentId => departmentId,
                    (item, departmentId) => item)
                .Distinct();
                if (includePermissionPredicate != null)
                {
                    return appliedAccessControlQuery.Union(query.Where(includePermissionPredicate)).Distinct();
                }

                return appliedAccessControlQuery;
            }
        }

        public static IQueryable<TEntity> ApplyAccessControl<TEntity>(
            this IQueryable<TEntity> query,
            IAccessControlContext<UserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePermissionPredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment
        {
            return ApplyAccessControl<TEntity, UserEntity>(query, accessControlContext, includePermissionPredicate);
        }

        public static IQueryable<TEntity> GetAllWithAccessControl<TEntity, TUserEntity>(
            this IReadOnlyRepository<TEntity> repository,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            return ApplyAccessControl(repository.GetAll(), accessControlContext, includePredicate);
        }

        public static IQueryable<TEntity> GetAllWithAccessControl<TEntity, TUserEntity>(
            this IRepository<TEntity> repository,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            return ApplyAccessControl(repository.GetAll(), accessControlContext, includePredicate);
        }

        public static Func<TEntity, bool> GetHasAdminRightChecker<TEntity, TUserEntity>(
            this IReadOnlyRepository<TEntity> repository,
            List<TEntity> entities,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePermissionPredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            var entitiesIds = entities.Select(p => p.Id).Distinct();
            return GetHasAdminRightChecker(
                repository,
                p => entitiesIds.Contains(p.Id),
                accessControlContext,
                includePermissionPredicate);
        }

        public static Func<TEntity, bool> GetHasAdminRightChecker<TEntity, TUserEntity>(
            this IReadOnlyRepository<TEntity> repository,
            Expression<Func<TEntity, bool>> entitiesPredicate,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePermissionPredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            if (accessControlContext.UserContext.CurrentUserId() == null)
            {
                return p => true;
            }

            if (!accessControlContext.UserContext.IsAdministrator())
            {
                return includePermissionPredicate != null ? includePermissionPredicate.Compile() : p => false;
            }

            if (accessControlContext.UserContext.IsSysAdministrator())
            {
                return p => true;
            }

            var appliedAccessControlEntitiesDic = repository
                .GetAll()
                .Where(entitiesPredicate)
                .Join(
                    accessControlContext.GetInferiorDepartmentIds(),
                    item => item.DepartmentId,
                    departmentId => departmentId,
                    (item, departmentId) => item)
                .Distinct()
                .ToDictionary(p => p.Id);
            return p =>
            {
                if (includePermissionPredicate != null && includePermissionPredicate.Compile()(p))
                {
                    return true;
                }

                return appliedAccessControlEntitiesDic.ContainsKey(p.Id);
            };
        }

        public static Func<TEntity, bool> GetHasAdminRightChecker<TEntity, TUserEntity>(
            this IReadOnlyRepository<TEntity> repository,
            IQueryable<TEntity> entities,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePermissionPredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            if (accessControlContext.UserContext.CurrentUserId() == null)
            {
                return p => true;
            }

            if (!accessControlContext.UserContext.IsAdministrator())
            {
                return includePermissionPredicate != null ? includePermissionPredicate.Compile() : p => false;
            }

            if (accessControlContext.UserContext.IsSysAdministrator())
            {
                return p => true;
            }

            var appliedAccessControlEntitiesDic = entities
                .Join(
                    accessControlContext.GetInferiorDepartmentIds(),
                    item => item.DepartmentId,
                    departmentId => departmentId,
                    (item, departmentId) => item)
                .Distinct()
                .ToDictionary(p => p.Id);
            return p =>
            {
                if (includePermissionPredicate != null && includePermissionPredicate.Compile()(p))
                {
                    return true;
                }

                return appliedAccessControlEntitiesDic.ContainsKey(p.Id);
            };
        }

        public static Func<TEntity, bool> GetHasAdminRightChecker<TEntity, TUserEntity>(
            this IReadOnlyRepository<TEntity> repository,
            TEntity entity,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            return GetHasAdminRightChecker(repository, new List<TEntity>() { entity }, accessControlContext, includePredicate);
        }

        public static Func<TEntity, bool> GetHasAdminRightChecker<TEntity, TUserEntity>(
            this IQueryable<TEntity> entitiesQuery,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePredicate = null)
            where TEntity : class, IEntity<Guid>, IHasDepartment where TUserEntity : IUserEntity
        {
            if (accessControlContext.UserContext.IsSysAdministrator())
            {
                return p => true;
            }

            var appliedAccessControlEntitiesDic = entitiesQuery
                .Join(
                    accessControlContext.GetInferiorDepartmentIds(),
                    item => item.DepartmentId,
                    departmentId => departmentId,
                    (item, departmentId) => item)
                .Distinct()
                .ToDictionary(p => p.Id);
            return p =>
            {
                if (includePredicate != null && includePredicate.Compile()(p))
                {
                    return true;
                }

                return appliedAccessControlEntitiesDic.ContainsKey(p.Id);
            };
        }
    }
}
