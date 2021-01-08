using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SaveCourseDepartmentLogic : BaseBusinessLogic
    {
        private readonly IRepository<DepartmentUnitTypeDepartment> _departmentUnitTypeDepartmentRepository;
        private readonly IRepository<CourseDepartment> _courseDepartmentRepository;
        private readonly IRepository<HierarchyDepartment> _hierarchyDepartmentRepository;

        public SaveCourseDepartmentLogic(
            IRepository<DepartmentUnitTypeDepartment> departmentUnitTypeDepartmentRepository,
            IRepository<CourseDepartment> courseDepartmentRepository,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _departmentUnitTypeDepartmentRepository = departmentUnitTypeDepartmentRepository;
            _courseDepartmentRepository = courseDepartmentRepository;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
        }

        public async Task Execute(SaveCourseDepartmentRequestDto requestDto)
        {
            var courseDepartment = await _courseDepartmentRepository.FirstOrDefaultAsync(u => u.DepartmentId == requestDto.DepartmentId);
            if (courseDepartment == null)
            {
                courseDepartment = new CourseDepartment()
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = requestDto.DepartmentId,
                    Name = requestDto.DepartmentName
                };
                await _courseDepartmentRepository.InsertAsync(courseDepartment);
            }
            else
            {
                SetDataForCourseDepartmentEntity(requestDto, courseDepartment);
                await _courseDepartmentRepository.UpdateAsync(courseDepartment);
            }

            if (requestDto.HierarchyInfo != null)
            {
                var hierarchyDepartment = await _hierarchyDepartmentRepository.FirstOrDefaultAsync(hd => hd.HierarchyDepartmentId == requestDto.HierarchyInfo.HdId);
                if (hierarchyDepartment == null)
                {
                    hierarchyDepartment = new HierarchyDepartment()
                    {
                        Id = Guid.NewGuid(),
                        DepartmentId = courseDepartment.DepartmentId,
                        HierarchyDepartmentId = requestDto.HierarchyInfo.HdId,
                        ParentId = requestDto.HierarchyInfo.ParentHdId,
                        Path = requestDto.HierarchyInfo.Path
                    };

                    await _hierarchyDepartmentRepository.InsertAsync(hierarchyDepartment);
                }
                else
                {
                    hierarchyDepartment.Path = requestDto.HierarchyInfo.Path;
                    hierarchyDepartment.ParentId = requestDto.HierarchyInfo.ParentHdId;
                    await _hierarchyDepartmentRepository.UpdateAsync(hierarchyDepartment);
                }
            }

            var departmentUnitTypeDepartment = await _departmentUnitTypeDepartmentRepository
                .FirstOrDefaultAsync(u => u.DepartmentId == requestDto.DepartmentId && u.DepartmentUnitTypeId == requestDto.TypeOfOrganizationUnits);
            if (departmentUnitTypeDepartment == null)
            {
                departmentUnitTypeDepartment = new DepartmentUnitTypeDepartment()
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = requestDto.DepartmentId,
                    DepartmentUnitTypeId = requestDto.TypeOfOrganizationUnits
                };
                await _departmentUnitTypeDepartmentRepository.InsertAsync(departmentUnitTypeDepartment);
            }
            else
            {
                departmentUnitTypeDepartment.DepartmentId = requestDto.DepartmentId;
                departmentUnitTypeDepartment.DepartmentUnitTypeId = requestDto.TypeOfOrganizationUnits;
                await _departmentUnitTypeDepartmentRepository.UpdateAsync(departmentUnitTypeDepartment);
            }
        }

        private static void SetDataForCourseDepartmentEntity(SaveCourseDepartmentRequestDto requestDto, CourseDepartment courseDepartment)
        {
            courseDepartment.Name = requestDto.DepartmentName;
        }
    }
}
