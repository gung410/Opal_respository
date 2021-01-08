using System;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class DepartmentAggregatedEntityModel : BaseAggregatedEntityModel
    {
        public Department Department { get; private set; }

        public Guid? DepartmentUnitType { get; private set; }

        public int? DepartmentLevelType { get; private set; }

        public static DepartmentAggregatedEntityModel Create(Department department, Guid? departmentUnitType, int? departmentLevelType)
        {
            return new DepartmentAggregatedEntityModel
            {
                Department = department,
                DepartmentLevelType = departmentLevelType,
                DepartmentUnitType = departmentUnitType
            };
        }
    }
}
