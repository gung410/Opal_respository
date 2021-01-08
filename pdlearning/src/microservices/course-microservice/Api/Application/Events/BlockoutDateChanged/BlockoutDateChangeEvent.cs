using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class BlockoutDateChangeEvent : BaseThunderEvent
    {
        public BlockoutDateChangeEvent(BlockoutDate blockoutDate, BlockoutDateChangeType changeType)
        {
            BlockoutDate = blockoutDate;
            ChangeType = changeType;
        }

        public BlockoutDate BlockoutDate { get; }

        public BlockoutDateChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.blockoutDate.{ChangeType.ToString().ToLower()}";
        }
    }
}
