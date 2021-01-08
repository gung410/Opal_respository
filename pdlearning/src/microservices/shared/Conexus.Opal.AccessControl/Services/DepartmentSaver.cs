using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Conexus.Opal.AccessControl.Services
{
    public static class DepartmentSaver
    {
        public static async Task SaveDepartment(
            SaveDepartmentRequestDto requestDto,
            IRepository<Department> departmentRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository)
        {
            var department = await departmentRepository.FirstOrDefaultAsync(u => u.DepartmentId == requestDto.DepartmentId);
            if (department == null)
            {
                department = new Department()
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = requestDto.DepartmentId
                };
                await departmentRepository.InsertAsync(department);
            }

            if (requestDto.HierarchyInfo != null)
            {
                var hierarchyDepartment = await hierarchyDepartmentRepository.FirstOrDefaultAsync(hd => hd.HierarchyDepartmentId == requestDto.HierarchyInfo.HdId);
                if (hierarchyDepartment == null)
                {
                    hierarchyDepartment = new HierarchyDepartment()
                    {
                        Id = Guid.NewGuid(),
                        DepartmentId = department.DepartmentId,
                        HierarchyDepartmentId = requestDto.HierarchyInfo.HdId,
                        ParentId = requestDto.HierarchyInfo.ParentHdId,
                        Path = requestDto.HierarchyInfo.Path
                    };

                    await hierarchyDepartmentRepository.InsertAsync(hierarchyDepartment);
                }
                else
                {
                    hierarchyDepartment.Path = requestDto.HierarchyInfo.Path;
                    hierarchyDepartment.ParentId = requestDto.HierarchyInfo.ParentHdId;
                    await hierarchyDepartmentRepository.UpdateAsync(hierarchyDepartment);
                }
            }
        }
    }
}
