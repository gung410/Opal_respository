using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AttendanceTrackingChangeEventHandler : OutboxOpalMqEventHandler<AttendanceTrackingChangeEvent, AttendanceTracking>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public AttendanceTrackingChangeEventHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IOptions<RabbitMQOptions> options,
            IOutboxQueue queue,
            IUserContext userContext,
            IThunderCqrs thunderCqrs) : base(options, userContext, queue)
        {
            _thunderCqrs = thunderCqrs;
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override AttendanceTracking GetMQMessagePayload(AttendanceTrackingChangeEvent @event)
        {
            return @event.AttendanceTracking;
        }

        protected override async Task HandleAsync(AttendanceTrackingChangeEvent @event, CancellationToken cancellationToken)
        {
            if (@event.ChangeType == AttendanceTrackingChangeType.Created || @event.ChangeType == AttendanceTrackingChangeType.Updated)
            {
                var registration = await _readRegistrationRepository.GetAll().FirstOrDefaultAsync(r => r.Id == @event.AttendanceTracking.RegistrationId, cancellationToken);
                if (@event.AttendanceTracking.Status == AttendanceTrackingStatus.Present &&
                    registration.LearningStatus == LearningStatus.NotStarted)
                {
                    await _thunderCqrs.SendCommand(
                        new UpdateRegistrationLearningInfoCommand
                        {
                            RegistrationIds = new List<Guid> { registration.Id },
                            LearningStatus = LearningStatus.InProgress
                        },
                        cancellationToken);
                }
            }

            await base.HandleAsync(@event, cancellationToken);
        }
    }
}
