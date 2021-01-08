using Conexus.Opal.AccessControl.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Conexus.Opal.AccessControl.Infrastructure
{
    public class AccessControlContext : GenericAccessControlContext<UserEntity>, IAccessControlContext
    {
        public AccessControlContext(
           IUserContext userContext,
           IRepository<UserEntity> userRepository,
           IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
           IRepository<DepartmentType> departmentTypeRepository,
           IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository) : base(userContext, userRepository, hierarchyDepartmentRepository, departmentTypeRepository, departmentTypeDepartmentRepository)
        {
        }
    }
}
