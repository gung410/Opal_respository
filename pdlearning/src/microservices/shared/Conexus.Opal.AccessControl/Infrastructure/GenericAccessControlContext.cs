using System;
using System.Linq;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Helpers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Conexus.Opal.AccessControl.Infrastructure
{
    public class GenericAccessControlContext<TUser> : IAccessControlContext<TUser> where TUser : class, IUserEntity
    {
        private readonly IRepository<HierarchyDepartment> _hierarchyDepartmentRepository;
        private readonly IRepository<TUser> _userRepository;
        private readonly IRepository<DepartmentType> _departmentTypeRepository;
        private readonly IRepository<DepartmentTypeDepartment> _departmentTypeDepartmentRepository;

        public GenericAccessControlContext(
            IUserContext userContext,
            IRepository<TUser> userRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
            IRepository<DepartmentType> departmentTypeRepository,
            IRepository<DepartmentTypeDepartment> departmentTypeDepartmentRepository)
        {
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _userRepository = userRepository;
            _departmentTypeRepository = departmentTypeRepository;
            _departmentTypeDepartmentRepository = departmentTypeDepartmentRepository;
            UserContext = userContext;
        }

        public IUserContext UserContext { get; internal set; }

        private Guid? UserId
        {
            get
            {
                var idString = UserContext.GetValue<string>(CommonUserContextKeys.UserId);
                return string.IsNullOrEmpty(idString) ? null : (Guid?)Guid.Parse(idString);
            }
        }

        public IQueryable<int> GetInferiorDepartmentIds()
        {
            return UserHierarchyHelper.GetInferiorDepartmentIds(
                UserId,
                _userRepository,
                _hierarchyDepartmentRepository,
                _departmentTypeRepository,
                _departmentTypeDepartmentRepository);
        }

        public int GetUserDepartment()
        {
            if (UserId == null)
            {
                return -1;
            }

            return _userRepository.Get(UserId.Value).DepartmentId;
        }
    }
}
