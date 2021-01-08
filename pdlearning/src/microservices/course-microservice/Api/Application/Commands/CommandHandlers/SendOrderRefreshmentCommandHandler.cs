using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SendOrderRefreshmentCommandHandler : BaseCommandHandler<SendOrderRefreshmentCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public SendOrderRefreshmentCommandHandler(
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(SendOrderRefreshmentCommand command, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(command.Base64Message) && command.SendToEmails != null)
            {
                await _thunderCqrs.SendEvents(
                    command.SendToEmails.Concat(command.EmailCC).ToList().Select(sendToEmail =>
                        new SendOrderRefreshmentNotifySpecificUsersEvent(
                            CurrentUserIdOrDefault,
                            new SendOrderRefreshmentNotifySpecificUsersPayload(),
                            new List<string> { sendToEmail },
                            command.Subject,
                            command.GetDecodedMessage())),
                    cancellationToken);
            }
        }
    }
}
