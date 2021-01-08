using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.process.updated")]
    public class LearningProcessUpdatedConsumer : ScopedOpalMessageConsumer<LearningProcessChangeMessage>
    {
        public async Task InternalHandleAsync(
            LearningProcessChangeMessage message,
            IThunderCqrs cqrs)
        {
            // Learner just send event when start learning
            await cqrs.SendCommand(new UpdateRegistrationLearningInfoCommand()
            {
                RegistrationIds = new List<Guid> { message.RegistrationId },
                LearningStatus = LearningStatus.InProgress,
                LearningContentProgress = message.ProgressMeasure
            });
        }
    }
}
