using System.Linq;
using Conexus.Opal.AccessControl.Entities;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.AccessControl.Infrastructure
{
    public interface IAccessControlContext<TUser> where TUser : IUserEntity
    {
        IUserContext UserContext { get; }

        int GetUserDepartment();

        /// <summary>
        /// Get all department ID followed hierarchy except school level.
        /// In case user belong to school level, the acction will return all followed hierarchy.
        /// </summary>
        /// <returns>Department Ids a queryable.</returns>
        IQueryable<int> GetInferiorDepartmentIds();
    }

    public interface IAccessControlContext : IAccessControlContext<UserEntity>
    {
    }
}
