using System;
using System.Linq;
using System.Linq.Expressions;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Service.Authentication;

namespace Microservice.StandaloneSurvey.Infrastructure
{
    public static class RepositoryHasDepartmentExtensions
    {
        // The same logic with RepositoryHasDepartmentExtensions in module AccessControl
        // but extend to support more admin roles (DivisionalTrainingCoordinator, SchoolStaffDeveloper) only available for Standalone survey.
        public static IQueryable<TEntity> ApplyAccessControlEx<TEntity, TUserEntity>(
            this IQueryable<TEntity> query,
            IAccessControlContext<TUserEntity> accessControlContext,
            Expression<Func<TEntity, bool>> includePredicate = null)
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
                return includePredicate != null ? query.Where(includePredicate) : query.Where(p => false);
            }

            var appliedAccessControlQuery = query
                .Join(
                    accessControlContext.GetInferiorDepartmentIds(),
                    item => item.DepartmentId,
                    departmentId => departmentId,
                    (item, departmentId) => item)
                .Distinct();
            if (includePredicate != null)
            {
                return appliedAccessControlQuery.Union(query.Where(includePredicate)).Distinct();
            }

            return appliedAccessControlQuery;
        }

        // Standalone survey module available for more admin roles.
        public static bool IsAdministratorEx(this IUserContext userContext)
        {
            return userContext.GetRoles().Any(p => p == UserRoles.SystemAdministrator ||
                                                   p == UserRoles.DivisionAdministrator ||
                                                   p == UserRoles.BranchAdministrator ||
                                                   p == UserRoles.SchoolAdministrator ||
                                                   p == UserRoles.DivisionalTrainingCoordinator ||
                                                   p == UserRoles.SchoolStaffDeveloper);
        }
    }
}
