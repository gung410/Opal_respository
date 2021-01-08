using Microservice.WebinarAutoscaler.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Queries
{
    public class GetBBBServerByInstanceIdQuery : BaseThunderQuery<BBBServerModel>
    {
        public string InstanceId { get; set; }
    }
}
