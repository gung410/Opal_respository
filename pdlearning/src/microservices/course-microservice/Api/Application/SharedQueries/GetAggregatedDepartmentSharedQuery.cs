using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetAggregatedDepartmentSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<Department> _readDepartmentRepository;
        private readonly IReadOnlyRepository<DepartmentUnitTypeDepartment> _readDepartmentUnitTypeDepartmentRepository;
        private readonly IReadOnlyRepository<DepartmentTypeDepartment> _readDepartmentLevelTypeDepartmentRepository;

        public GetAggregatedDepartmentSharedQuery(
            IReadOnlyRepository<Department> readDepartmentRepository,
            IReadOnlyRepository<DepartmentUnitTypeDepartment> readDepartmentUnitTypeDepartmentRepository,
            IReadOnlyRepository<DepartmentTypeDepartment> readDepartmentLevelTypeDepartmentRepository)
        {
            _readDepartmentRepository = readDepartmentRepository;
            _readDepartmentUnitTypeDepartmentRepository = readDepartmentUnitTypeDepartmentRepository;
            _readDepartmentLevelTypeDepartmentRepository = readDepartmentLevelTypeDepartmentRepository;
        }

        public async Task<List<DepartmentAggregatedEntityModel>> ByDepartmentIds(
            List<int> departmentIds,
            CancellationToken cancellationToken)
        {
            var departments = await _readDepartmentRepository.GetAll().Where(p => departmentIds.Contains(p.DepartmentId)).ToListAsync(cancellationToken);

            var departmentUnitTypeDepartments = await _readDepartmentUnitTypeDepartmentRepository
                .GetAll()
                .Where(p => departmentIds.Contains(p.DepartmentId))
                .ToListAsync(cancellationToken);

            var departmentIdToDepartmentUnitTypeDic = departmentUnitTypeDepartments
                .GroupBy(p => p.DepartmentId)
                .ToDictionary(p => p.Key, p => (Guid?)p.First().DepartmentUnitTypeId);

            var departmentIdToDepartmentLevelType = await _readDepartmentLevelTypeDepartmentRepository
                .GetAll()
                .Where(p => departmentIds.Contains(p.DepartmentId))
                .ToListAsync(cancellationToken);

            var departmentIdToDepartmentLevelTypeDic = departmentIdToDepartmentLevelType
                .GroupBy(p => p.DepartmentId)
                .ToDictionary(p => p.Key, p => (int?)p.First().DepartmentTypeId);

            return departments
                .Select(p => DepartmentAggregatedEntityModel.Create(
                    p,
                    departmentIdToDepartmentUnitTypeDic.GetValueOrDefault(p.DepartmentId),
                    departmentIdToDepartmentLevelTypeDic.GetValueOrDefault(p.DepartmentId)))
                .ToList();
        }
    }
}
