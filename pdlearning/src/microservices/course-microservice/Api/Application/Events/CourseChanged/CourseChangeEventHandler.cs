using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class CourseChangeEventHandler : OutboxOpalMqEventHandler<CourseChangeEvent, CourseEntity>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public CourseChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext, IThunderCqrs thunderCqrs) : base(options, userContext, queue)
        {
            _thunderCqrs = thunderCqrs;
        }

        protected override CourseEntity GetMQMessagePayload(CourseChangeEvent @event)
        {
            return @event.Course;
        }

        protected override async Task HandleAsync(CourseChangeEvent @event, CancellationToken cancellationToken)
        {
            if (@event.ChangeType == CourseChangeType.Deleted)
            {
                await _thunderCqrs.SendCommand(
                    new DeleteLearningPathCourseByCourseIdCommand
                    {
                        CourseId = @event.Course.Id
                    },
                    cancellationToken);
            }

            await base.HandleAsync(@event, cancellationToken);
        }
    }
}
