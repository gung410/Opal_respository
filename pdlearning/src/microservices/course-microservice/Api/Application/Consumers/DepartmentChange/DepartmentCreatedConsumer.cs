using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.RequestDtos;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("cx-organization-api.crud.created.organizationalunit")]
    public class DepartmentCreatedConsumer : ScopedOpalMessageConsumer<DepartmentChangeMessage>
    {
        public async Task InternalHandleAsync(
            DepartmentChangeMessage message,
            SaveCourseDepartmentLogic saveCourseDepartmentLogic)
        {
            await saveCourseDepartmentLogic.Execute(
                new SaveCourseDepartmentRequestDto
                {
                    DepartmentId = message.DepartmentId,
                    DepartmentName = message.DepartmentData.Name,
                    HierarchyInfo = message.HierarchyInfo != null
                    ? new SaveCourseDepartmentRequestDtoHierarchyInfo
                    {
                        HdId = message.HierarchyInfo.HdId,
                        HierarchyId = message.HierarchyInfo.HierarchyId,
                        ParentHdId = message.HierarchyInfo.ParentHdId,
                        Path = message.HierarchyInfo.Path,
                    }
                    : null,
                    TypeOfOrganizationUnits = message.DepartmentData.JsonDynamicAttributes.TypeOfOrganizationUnits
                });
        }
    }
}
