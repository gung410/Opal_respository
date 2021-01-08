using System;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.RequestDtos;
using Conexus.Opal.AccessControl.Services;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.organizationalunit")]
    public class DepartmentCreatedConsumer : ScopedOpalMessageConsumer<DepartmentChangeMessage>
    {
        public async Task InternalHandleAsync(
            DepartmentChangeMessage message,
            IRepository<Department> departmentRepo,
            IRepository<HierarchyDepartment> hierarchyDepartmentRepo)
        {
            await DepartmentSaver.SaveDepartment(
                new SaveDepartmentRequestDto
                {
                    DepartmentId = message.DepartmentId,
                    HierarchyInfo = message.HierarchyInfo != null
                    ? new SaveDepartmentRequestDtoHierarchyInfo
                    {
                        HdId = message.HierarchyInfo.HdId,
                        HierarchyId = message.HierarchyInfo.HierarchyId,
                        ParentHdId = message.HierarchyInfo.ParentHdId,
                        Path = message.HierarchyInfo.Path,
                    }
                    : null
                },
                departmentRepo,
                hierarchyDepartmentRepo);
        }
    }
}
