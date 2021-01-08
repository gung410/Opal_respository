using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Conexus.Opal.AccessControl.Services;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SaveDepartmentLogic : BaseBusinessLogic
    {
        private readonly IRepository<DepartmentUnitTypeDepartment> _departmentUnitTypeDepartmentRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<HierarchyDepartment> _hierarchyDepartmentRepository;

        public SaveDepartmentLogic(
            IRepository<DepartmentUnitTypeDepartment> departmentUnitTypeDepartmentRepository,
            IRepository<Department> departmentRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _departmentUnitTypeDepartmentRepository = departmentUnitTypeDepartmentRepository;
            _departmentRepository = departmentRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
        }

        public async Task Execute(SaveDepartmentRequestDto request)
        {
            await DepartmentSaver.SaveDepartment(
                request,
                _departmentRepository,
                _hierarchyDepartmentRepository);

            var departmentUnitTypeDepartment = await _departmentUnitTypeDepartmentRepository
                .FirstOrDefaultAsync(u => u.DepartmentId == request.DepartmentId && u.DepartmentUnitTypeId == request.TypeOfOrganizationUnits);
            if (departmentUnitTypeDepartment == null)
            {
                departmentUnitTypeDepartment = new DepartmentUnitTypeDepartment()
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = request.DepartmentId,
                    DepartmentUnitTypeId = request.TypeOfOrganizationUnits
                };
                await _departmentUnitTypeDepartmentRepository.InsertAsync(departmentUnitTypeDepartment);
            }
            else
            {
                departmentUnitTypeDepartment.DepartmentId = request.DepartmentId;
                departmentUnitTypeDepartment.DepartmentUnitTypeId = request.TypeOfOrganizationUnits;
                await _departmentUnitTypeDepartmentRepository.UpdateAsync(departmentUnitTypeDepartment);
            }
        }
    }
}
