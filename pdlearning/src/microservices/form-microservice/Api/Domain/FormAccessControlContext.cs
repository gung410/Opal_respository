using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Form.Domain
{
    public class FormAccessControlContext : GenericAccessControlContext<FormUser>, IAccessControlContext
    {
        public FormAccessControlContext(
           IUserContext userContext,
           IRepository<FormUser> userRepository,
           IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
           IRepository<DepartmentType> departmentTypeRepository,
           IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository) : base(userContext, userRepository, hierarchyDepartmentRepository, departmentTypeRepository, departmentTypeDepartmentRepository)
        {
        }
    }
}
